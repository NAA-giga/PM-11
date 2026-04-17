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
                        SelectedEmployee = null;
                        RequestModel.EmployeeId = null;
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
                        RequestModel.EmployeeId = value.Id;
                        RequestModel.EmployeeFullName = value.FullName;
                    }
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

            // Минимальная дата - завтрашний день (1 день вперед)
            DateTime tomorrow = DateTime.Now.AddDays(1);

            RequestModel = new RequestModel
            {
                StartDate = tomorrow,
                EndDate = tomorrow.AddDays(1),
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
            }
        }

        private bool CanSubmitRequest()
        {
            // Минимальная дата начала - завтра
            DateTime minStartDate = DateTime.Now.AddDays(1);

            // Максимальная дата окончания = дата начала + 15 дней
            DateTime maxEndDate = RequestModel.StartDate.AddDays(15);

            bool condition1 = !IsLoading;
            bool condition2 = !IsLoadingEmployees;
            bool condition3 = RequestModel != null;

            // Дата начала должна быть >= завтра
            bool condition4 = RequestModel.StartDate >= minStartDate;

            // Дата окончания должна быть >= даты начала
            bool condition5 = RequestModel.EndDate >= RequestModel.StartDate;

            // Дата окончания должна быть <= дата начала + 15 дней
            bool condition6 = RequestModel.EndDate <= maxEndDate;

            bool condition7 = !string.IsNullOrWhiteSpace(RequestModel.Purpose);
            bool condition8 = RequestModel.DepartmentId.HasValue;
            bool condition9 = RequestModel.EmployeeId.HasValue;

            // Отладка (можно удалить после проверки)
            System.Diagnostics.Debug.WriteLine($"CanSubmitRequest: StartDate={RequestModel.StartDate:dd.MM.yyyy}, MinStartDate={minStartDate:dd.MM.yyyy}, Condition4={condition4}");
            System.Diagnostics.Debug.WriteLine($"CanSubmitRequest: EndDate={RequestModel.EndDate:dd.MM.yyyy}, MaxEndDate={maxEndDate:dd.MM.yyyy}, Condition6={condition6}");

            return condition1 && condition2 && condition3 && condition4 && condition5 &&
                   condition6 && condition7 && condition8 && condition9;
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
