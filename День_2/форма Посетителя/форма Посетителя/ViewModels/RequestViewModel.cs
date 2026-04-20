using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using форма_Посетителя.Helpers;
using форма_Посетителя.Models;
using форма_Посетителя.Services;
using форма_Посетителя.Views;

namespace форма_Посетителя.ViewModels
{
    public class RequestViewModel : BaseViewModel
    {
        private readonly IRequestService _requestService;

        private RequestModel _requestModel;
        public RequestModel RequestModel
        {
            get => _requestModel;
            set => SetProperty(ref _requestModel, value);
        }

        private ObservableCollection<DepartmentItem> _departments;
        public ObservableCollection<DepartmentItem> Departments
        {
            get => _departments;
            set => SetProperty(ref _departments, value);
        }

        private ObservableCollection<EmployeeItem> _employees;
        public ObservableCollection<EmployeeItem> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        private DepartmentItem? _selectedDepartment;
        public DepartmentItem? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if (SetProperty(ref _selectedDepartment, value))
                {
                    if (value != null)
                    {
                        RequestModel.DepartmentId = value.Id;
                        RequestModel.DepartmentName = value.Name;
                        RequestModel.EmployeeId = null;
                        SelectedEmployee = null;
                        LoadEmployees(value.Id);
                    }
                }
            }
        }

        private EmployeeItem? _selectedEmployee;
        public EmployeeItem? SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (SetProperty(ref _selectedEmployee, value))
                {
                    if (value != null)
                    {
                        // Прямое присвоение ID в RequestModel
                        RequestModel.EmployeeId = value.Id;
                        RequestModel.EmployeeFullName = value.FullName;
                        System.Diagnostics.Debug.WriteLine($"EmployeeId установлен в RequestModel: {RequestModel.EmployeeId}");
                    }
                    else
                    {
                        RequestModel.EmployeeId = null;
                    }
                    // Принудительно обновляем состояние кнопки
                    (SubmitRequestCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
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

        private bool _isLoadingEmployees = false;
        public bool IsLoadingEmployees
        {
            get => _isLoadingEmployees;
            set => SetProperty(ref _isLoadingEmployees, value);
        }

        public ICommand SubmitRequestCommand { get; }
        public ICommand SwitchToGroupCommand { get; }
        public ICommand GoToMainCommand { get; }

        public RequestViewModel(IRequestService requestService)
        {
            _requestService = requestService;

            RequestModel = new RequestModel
            {
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2),
                Purpose = "",
                Note = ""
            };

            var currentUser = App.CurrentUser;
            if (currentUser != null)
            {
                RequestModel.LastName = currentUser.Фамилия ?? "";
                RequestModel.FirstName = currentUser.Имя ?? "";
                RequestModel.MiddleName = currentUser.Отчество ?? "";
                RequestModel.Phone = currentUser.Телефон ?? "";
                RequestModel.Email = currentUser.Email ?? "";
                RequestModel.BirthDate = currentUser.ДатаРождения.HasValue
                    ? currentUser.ДатаРождения.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null;
                RequestModel.PassportSeries = currentUser.СерияПаспорта ?? "";
                RequestModel.PassportNumber = currentUser.НомерПаспорта ?? "";
                RequestModel.Organization = currentUser.Организация ?? "";
            }

            Departments = new ObservableCollection<DepartmentItem>();
            Employees = new ObservableCollection<EmployeeItem>();

            SubmitRequestCommand = new RelayCommand(async _ => await SubmitRequest(), _ => CanSubmitRequest());
            SwitchToGroupCommand = new RelayCommand(_ => SwitchToGroup());
            GoToMainCommand = new RelayCommand(_ => GoToMain());

            LoadDepartments();
        }

        private async void LoadDepartments()
        {
            IsLoading = true;
            try
            {
                var depts = await _requestService.GetDepartments();
                Departments.Clear();
                foreach (var dept in depts)
                {
                    Departments.Add(dept);
                }

                if (Departments.Count > 0)
                {
                    StatusMessage = $"Загружено {Departments.Count} подразделений";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки подразделений: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки подразделений: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
                (SubmitRequestCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private async void LoadEmployees(int departmentId)
        {
            IsLoadingEmployees = true;
            try
            {
                var empls = await _requestService.GetEmployeesByDepartment(departmentId);

                Employees.Clear();

                if (empls != null && empls.Count > 0)
                {
                    foreach (var emp in empls)
                    {
                        Employees.Add(emp);
                    }
                    StatusMessage = $"Загружено {Employees.Count} сотрудников";
                }
                else
                {
                    StatusMessage = "В выбранном подразделении нет сотрудников";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки сотрудников: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoadingEmployees = false;
                (SubmitRequestCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private bool CanSubmitRequest()
        { 
            return true;
        }

        private async Task SubmitRequest()
        {
            IsLoading = true;
            StatusMessage = "Создание заявки...";

            try
            {
                if (App.CurrentUser == null)
                {
                    StatusMessage = "Пользователь не авторизован";
                    MessageBox.Show("Пользователь не авторизован. Пожалуйста, войдите снова.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    GoToMain();
                    return;
                }

                // Отладка перед отправкой
                System.Diagnostics.Debug.WriteLine($"SubmitRequest: DepartmentId={RequestModel.DepartmentId}, EmployeeId={RequestModel.EmployeeId}");

                var result = await _requestService.CreateIndividualRequest(RequestModel, App.CurrentUser.Id);

                if (result.Success)
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    GoToMain();
                }
                else
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void SwitchToGroup()
        {
            var groupRequestWindow = new GroupRequestWindow();
            groupRequestWindow.Show();
            Application.Current.Windows[0]?.Close();
        }

        private void GoToMain()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Current.Windows[0]?.Close();
        }
    }
}
