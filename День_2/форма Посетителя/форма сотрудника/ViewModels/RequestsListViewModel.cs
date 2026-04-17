using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using форма_сотрудника.Helpers;
using форма_сотрудника.Models;
using форма_сотрудника.Services;
using форма_сотрудника.Views;

namespace форма_сотрудника.ViewModels
{
    public class RequestsListViewModel : BaseViewModel
    {
        private readonly IRequestService _requestService;
        private readonly bool _isMyRequests;

        private ObservableCollection<Заявка> _requests;
        public ObservableCollection<Заявка> Requests
        {
            get => _requests;
            set => SetProperty(ref _requests, value);
        }

        private Заявка _selectedRequest;
        public Заявка SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                SetProperty(ref _selectedRequest, value);
                // Обновляем состояние кнопок при выборе заявки
                (ViewDetailsCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ApproveRequestCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RejectRequestCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _filterStatus = "Все";
        public string FilterStatus
        {
            get => _filterStatus;
            set
            {
                if (SetProperty(ref _filterStatus, value))
                {
                    _ = LoadRequests();
                }
            }
        }

        public ObservableCollection<string> StatusFilters { get; set; }

        public ICommand ViewDetailsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand GoToMainCommand { get; }
        public ICommand ApproveRequestCommand { get; }
        public ICommand RejectRequestCommand { get; }

        public RequestsListViewModel(IRequestService requestService, bool isMyRequests)
        {
            _requestService = requestService;
            _isMyRequests = isMyRequests;

            Requests = new ObservableCollection<Заявка>();

            StatusFilters = new ObservableCollection<string>
            {
                "Все",
                "проверка",
                "одобрена",
                "не одобрена"
            };

            // Кнопка "Детали" активна при выборе заявки
            ViewDetailsCommand = new RelayCommand(_ => ViewDetails(), _ => SelectedRequest != null);
            RefreshCommand = new RelayCommand(async _ => await LoadRequests());
            GoToMainCommand = new RelayCommand(_ => GoToMain());
            ApproveRequestCommand = new RelayCommand(async _ => await ApproveRequest(), _ => CanApprove());
            RejectRequestCommand = new RelayCommand(async _ => await RejectRequest(), _ => CanReject());

            _ = LoadRequests();
        }

        private bool CanApprove()
        {
            return SelectedRequest != null && SelectedRequest.Status == "проверка";
        }

        private bool CanReject()
        {
            return SelectedRequest != null && SelectedRequest.Status == "проверка";
        }

        public async Task LoadRequests()
        {
            IsLoading = true;
            StatusMessage = "Загрузка заявок...";

            try
            {
                if (App.CurrentEmployee == null)
                {
                    StatusMessage = "Сотрудник не авторизован";
                    return;
                }

                System.Collections.Generic.List<Заявка> allRequests;

                if (_isMyRequests)
                {
                    allRequests = await _requestService.GetRequestsByEmployee(App.CurrentEmployee.Id);
                }
                else
                {
                    int departmentId = App.CurrentEmployee.DepartmentId ?? 0;
                    allRequests = await _requestService.GetRequestsByDepartment(departmentId);
                }

                Requests.Clear();

                foreach (var request in allRequests)
                {
                    if (FilterStatus == "Все" || request.Status == FilterStatus)
                    {
                        Requests.Add(request);
                    }
                }

                StatusMessage = $"Найдено заявок: {Requests.Count}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ViewDetails()
        {
            if (SelectedRequest != null)
            {
                var detailsWindow = new RequestDetailsWindow(SelectedRequest.Id);
                detailsWindow.ShowDialog();
                _ = LoadRequests();
            }
        }

        private async Task ApproveRequest()
        {
            if (SelectedRequest == null) return;

            var result = MessageBox.Show($"Одобрить заявку #{SelectedRequest.Id} от {SelectedRequest.Visitor?.LastName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true;
                try
                {
                    var response = await _requestService.ApproveRequest(SelectedRequest.Id, App.CurrentEmployee.Id);

                    if (response.Success)
                    {
                        MessageBox.Show(response.Message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadRequests();
                    }
                    else
                    {
                        MessageBox.Show(response.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private async Task RejectRequest()
        {
            if (SelectedRequest == null) return;

            var reasonDialog = new InputDialog("Введите причину отказа:", "Отказ в заявке");
            if (reasonDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(reasonDialog.Answer))
            {
                IsLoading = true;
                try
                {
                    var response = await _requestService.RejectRequest(SelectedRequest.Id, App.CurrentEmployee.Id, reasonDialog.Answer);

                    if (response.Success)
                    {
                        MessageBox.Show(response.Message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadRequests();
                    }
                    else
                    {
                        MessageBox.Show(response.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private void GoToMain()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Current.Windows[0]?.Close();
        }
    }
}
