using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using форма_для_сотрудника_охраны.Helpers;
using форма_для_сотрудника_охраны.Models;
using форма_для_сотрудника_охраны.Services;
using форма_для_сотрудника_охраны.Views;

namespace форма_для_сотрудника_охраны.ViewModels
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
                (OpenAccessCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DepartureCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
        private DateTime? _filterDate;
        public DateTime? FilterDate
        {
            get => _filterDate;
            set
            {
                SetProperty(ref _filterDate, value);
                ApplyFilters();
            }
        }

        private string _filterRequestType = "Все";
        public string FilterRequestType
        {
            get => _filterRequestType;
            set
            {
                SetProperty(ref _filterRequestType, value);
                ApplyFilters();
            }
        }

        private DepartmentItem? _filterDepartment;
        public DepartmentItem? FilterDepartment
        {
            get => _filterDepartment;
            set
            {
                SetProperty(ref _filterDepartment, value);
                ApplyFilters();
            }
        }

        public ObservableCollection<string> RequestTypes { get; set; }
        public ObservableCollection<DepartmentItem> Departments { get; set; }

        public ICommand OpenAccessCommand { get; }
        public ICommand DepartureCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand GoToMainCommand { get; }

        public RequestsListViewModel(IRequestService requestService)
        {
            _requestService = requestService;

            Requests = new ObservableCollection<Заявка>();
            Departments = new ObservableCollection<DepartmentItem>();

            RequestTypes = new ObservableCollection<string> { "Все", "личная", "групповая" };

            OpenAccessCommand = new RelayCommand(_ => OpenAccess(), _ => SelectedRequest != null);
            DepartureCommand = new RelayCommand(_ => OpenDeparture(), _ => SelectedRequest != null);
            RefreshCommand = new RelayCommand(async _ => await LoadRequests());
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
                if (App.CurrentEmployee == null)
                {
                    StatusMessage = "Сотрудник не авторизован";
                    return;
                }

                _allRequests = await _requestService.GetApprovedRequests();

                ApplyFilters();

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

        private void ApplyFilters()
        {
            var filtered = _allRequests.AsEnumerable();

            if (FilterDate.HasValue)
            {
                var targetDate = DateOnly.FromDateTime(FilterDate.Value);
                filtered = filtered.Where(r => r.StartDate == targetDate);
            }

            if (FilterRequestType != "Все")
            {
                filtered = filtered.Where(r => r.RequestType == FilterRequestType);
            }

            if (FilterDepartment != null && FilterDepartment.Id > 0)
            {
                filtered = filtered.Where(r => r.DepartmentId == FilterDepartment.Id);
            }

            Requests.Clear();
            foreach (var request in filtered)
            {
                Requests.Add(request);
            }
        }

        private void OpenAccess()
        {
            if (SelectedRequest != null)
            {
                var accessWindow = new AccessControlWindow(SelectedRequest.Id);
                accessWindow.ShowDialog();
                LoadRequests();
            }
        }

        private void OpenDeparture()
        {
            if (SelectedRequest != null)
            {
                var departureWindow = new DepartureWindow(SelectedRequest.Id);
                departureWindow.ShowDialog();
                LoadRequests();
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
