using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using форма_Посетителя.Helpers;
using форма_Посетителя.Models;
using форма_Посетителя.Services;
using форма_Посетителя.Views;

namespace форма_Посетителя.ViewModels
{
    public class RequestsListViewModel : BaseViewModel
    {
        private readonly IRequestService _requestService;

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
            set => SetProperty(ref _selectedRequest, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _filterStatus;
        public string FilterStatus
        {
            get => _filterStatus;
            set
            {
                if (SetProperty(ref _filterStatus, value))
                {
                    _ = LoadRequestsAsync();
                }
            }
        }

        public ObservableCollection<string> StatusFilters { get; set; }

        public ICommand ViewDetailsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand GoToMainCommand { get; }
        public ICommand NewRequestCommand { get; }

        public RequestsListViewModel(IRequestService requestService)
        {
            _requestService = requestService;

            Requests = new ObservableCollection<Заявка>();

            StatusFilters = new ObservableCollection<string>
            {
                "Все",
                "проверка",
                "одобрена",
                "не одобрена"
            };
            FilterStatus = "Все";

            ViewDetailsCommand = new RelayCommand(_ => ViewDetails(), _ => SelectedRequest != null);
            RefreshCommand = new RelayCommand(async _ => await LoadRequestsAsync());
            GoToMainCommand = new RelayCommand(_ => GoToMain());
            NewRequestCommand = new RelayCommand(_ => NewRequest());

            // Загружаем заявки асинхронно
            _ = LoadRequestsAsync();
        }

        public async Task LoadRequestsAsync()
        {
            IsLoading = true;
            StatusMessage = "Загрузка заявок...";

            try
            {
                if (App.CurrentUser == null)
                {
                    StatusMessage = "Пользователь не авторизован";
                    return;
                }

                var allRequests = await _requestService.GetVisitorRequests(App.CurrentUser.Id);

                Requests.Clear();

                foreach (var request in allRequests)
                {
                    if (FilterStatus == "Все" || request.Статус == FilterStatus)
                    {
                        Requests.Add(request);
                    }
                }

                StatusMessage = $"📋 Найдено {Requests.Count} заявок";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Ошибка загрузки: {ex.Message}";
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
                _ = LoadRequestsAsync(); // Обновляем список после закрытия
            }
        }

        private void GoToMain()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Current.Windows[0]?.Close();
        }

        private void NewRequest()
        {
            var requestWindow = new RequestWindow();
            requestWindow.Show();
            Application.Current.Windows[0]?.Close();
        }
    }
}
