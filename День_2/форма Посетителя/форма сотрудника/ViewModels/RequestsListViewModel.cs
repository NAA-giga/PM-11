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
        private List<Заявка> _allRequests = new List<Заявка>();

        private ObservableCollection<Заявка> _requests = new ObservableCollection<Заявка>();
        public ObservableCollection<Заявка> Requests
        {
            get => _requests;
            set => SetProperty(ref _requests, value);
        }

        private Заявка? _selectedRequest;
        public Заявка? SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                SetProperty(ref _selectedRequest, value);
                // Обновляем состояние кнопки при выборе заявки
                System.Diagnostics.Debug.WriteLine($"SelectedRequest changed: {value?.Id} - {value?.Name}");
                (ReviewRequestCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

        // Фильтры
        private string _filterRequestType = "Все";
        public string FilterRequestType
        {
            get => _filterRequestType;
            set => SetProperty(ref _filterRequestType, value);
        }

        private DepartmentItem? _filterDepartment;
        public DepartmentItem? FilterDepartment
        {
            get => _filterDepartment;
            set => SetProperty(ref _filterDepartment, value);
        }

        private string _filterStatus = "Все";
        public string FilterStatus
        {
            get => _filterStatus;
            set => SetProperty(ref _filterStatus, value);
        }

        public ObservableCollection<string> RequestTypes { get; set; }
        public ObservableCollection<string> StatusFilters { get; set; }
        public ObservableCollection<DepartmentItem> Departments { get; set; }

        public ICommand ReviewRequestCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ResetFiltersCommand { get; }
        public ICommand GoToMainCommand { get; }

        public RequestsListViewModel(IRequestService requestService)
        {
            _requestService = requestService;

            Requests = new ObservableCollection<Заявка>();
            Departments = new ObservableCollection<DepartmentItem>();

            RequestTypes = new ObservableCollection<string> { "Все", "личная", "групповая" };
            StatusFilters = new ObservableCollection<string> { "Все", "проверка", "одобрена", "не одобрена" };

            ReviewRequestCommand = new RelayCommand(_ => ReviewRequest(), _ => true);
            RefreshCommand = new RelayCommand(async _ => await LoadRequests());
            ApplyFiltersCommand = new RelayCommand(_ => ApplyFilters());
            ResetFiltersCommand = new RelayCommand(_ => ResetFilters());
            GoToMainCommand = new RelayCommand(_ => GoToMain());

            LoadDepartments();
            LoadRequests();
        }

        private async void LoadDepartments()
        {
            try
            {
                var depts = await _requestService.GetDepartments();
                Departments.Clear();
                Departments.Add(new DepartmentItem { Id = 0, Name = "Все" });
                foreach (var dept in depts)
                {
                    Departments.Add(dept);
                }
                FilterDepartment = Departments.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки подразделений: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task LoadRequests()
        {
            IsLoading = true;
            StatusMessage = "Загрузка заявок...";

            try
            {
                _allRequests = await _requestService.GetAllPendingRequests();

                System.Diagnostics.Debug.WriteLine($"Загружено заявок: {_allRequests.Count}");

                ApplyFilters();

                System.Diagnostics.Debug.WriteLine($"После фильтра: {Requests.Count}");

                StatusMessage = $"Найдено заявок: {Requests.Count}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allRequests.AsEnumerable();

            if (FilterRequestType != "Все")
            {
                filtered = filtered.Where(r => r.RequestType == FilterRequestType);
            }

            if (FilterDepartment != null && FilterDepartment.Id > 0)
            {
                filtered = filtered.Where(r => r.DepartmentId == FilterDepartment.Id);
            }

            if (FilterStatus != "Все")
            {
                filtered = filtered.Where(r => r.Status == FilterStatus);
            }

            Requests.Clear();
            foreach (var request in filtered)
            {
                Requests.Add(request);
            }
        }

        private void ResetFilters()
        {
            FilterRequestType = "Все";
            FilterDepartment = Departments.FirstOrDefault(d => d.Name == "Все");
            FilterStatus = "Все";
            ApplyFilters();
        }

        private void ReviewRequest()
        {
            if (SelectedRequest != null)
            {
                var reviewWindow = new RequestReviewWindow(SelectedRequest.Id);
                reviewWindow.ShowDialog();
                LoadRequests();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заявку для проверки.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
