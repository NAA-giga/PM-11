using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using форма_Посетителя.Helpers;
using форма_Посетителя.Models;
using форма_Посетителя.Services;
using форма_Посетителя.Views;

namespace форма_Посетителя.ViewModels
{
    public class GroupRequestViewModel : BaseViewModel
    {
        private readonly IRequestService _requestService;
        private readonly IExcelService _excelService;

        private GroupRequestModel _requestModel;
        public GroupRequestModel RequestModel
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

        private ObservableCollection<GroupVisitorModel> _visitors;
        public ObservableCollection<GroupVisitorModel> Visitors
        {
            get => _visitors;
            set => SetProperty(ref _visitors, value);
        }

        private DepartmentItem _selectedDepartment;
        public DepartmentItem SelectedDepartment
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
                        LoadEmployees(value.Id);
                    }
                }
            }
        }

        private EmployeeItem _selectedEmployee;
        public EmployeeItem SelectedEmployee
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

        private bool _hasVisitors;
        public bool HasVisitors
        {
            get => _hasVisitors;
            set => SetProperty(ref _hasVisitors, value);
        }

        public ICommand DownloadTemplateCommand { get; }
        public ICommand ImportExcelCommand { get; }
        public ICommand SubmitRequestCommand { get; }
        public ICommand GoToMainCommand { get; }
        public ICommand SwitchToIndividualCommand { get; }

        public GroupRequestViewModel(IRequestService requestService, IExcelService excelService)
        {
            _requestService = requestService;
            _excelService = excelService;

            RequestModel = new GroupRequestModel
            {
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2),
                Purpose = "",
                Note = ""
            };

            var currentUser = App.CurrentUser;
            if (currentUser != null)
            {
                RequestModel.LastName = currentUser.Фамилия;
                RequestModel.FirstName = currentUser.Имя;
                RequestModel.MiddleName = currentUser.Отчество;
                RequestModel.Phone = currentUser.Телефон;
                RequestModel.Email = currentUser.Email;
                RequestModel.BirthDate = currentUser.ДатаРождения.HasValue
                    ? currentUser.ДатаРождения.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null;
                RequestModel.PassportSeries = currentUser.СерияПаспорта;
                RequestModel.PassportNumber = currentUser.НомерПаспорта;
                RequestModel.Organization = currentUser.Организация;
            }

            Departments = new ObservableCollection<DepartmentItem>();
            Employees = new ObservableCollection<EmployeeItem>();
            Visitors = new ObservableCollection<GroupVisitorModel>();

            DownloadTemplateCommand = new RelayCommand(_ => DownloadTemplate());
            ImportExcelCommand = new RelayCommand(async _ => await ImportExcel());
            SubmitRequestCommand = new RelayCommand(async _ => await SubmitRequest(), _ => CanSubmitRequest());
            GoToMainCommand = new RelayCommand(_ => GoToMain());
            SwitchToIndividualCommand = new RelayCommand(_ => SwitchToIndividual());

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
            }
            catch (Exception ex)
            {
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
            IsLoading = true;
            try
            {
                var empls = await _requestService.GetEmployeesByDepartment(departmentId);
                Employees.Clear();
                foreach (var emp in empls)
                {
                    Employees.Add(emp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void DownloadTemplate()
        {
            try
            {
                var template = _excelService.GenerateTemplate();
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = "Шаблон_списка_посетителей.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveDialog.FileName, template);
                    MessageBox.Show("Шаблон успешно сохранён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении шаблона: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ImportExcel()
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Выберите файл со списком посетителей"
            };

            if (openDialog.ShowDialog() == true)
            {
                IsLoading = true;
                StatusMessage = "Загрузка данных из Excel...";

                try
                {
                    var result = await _excelService.ImportVisitorsFromExcel(openDialog.FileName);

                    if (result.Success)
                    {
                        Visitors.Clear();
                        foreach (var visitor in result.Visitors)
                        {
                            Visitors.Add(visitor);
                        }
                        HasVisitors = Visitors.Count > 0;
                        StatusMessage = $"Загружено {Visitors.Count} посетителей";
                        MessageBox.Show($"Успешно загружено {Visitors.Count} посетителей!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = result.ErrorMessage;
                        MessageBox.Show(result.ErrorMessage, "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                    MessageBox.Show($"Ошибка при импорте: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private bool CanSubmitRequest()
        {
            return !IsLoading && RequestModel != null &&
                   RequestModel.StartDate >= DateTime.Now.AddDays(1) &&
                   RequestModel.EndDate >= RequestModel.StartDate &&
                   RequestModel.EndDate <= RequestModel.StartDate.AddDays(15) &&
                   !string.IsNullOrWhiteSpace(RequestModel.Purpose) &&
                   RequestModel.DepartmentId.HasValue &&
                   RequestModel.EmployeeId.HasValue &&
                   Visitors.Count >= 5;  // Группа не менее 5 человек
        }

        private async Task SubmitRequest()
        {
            IsLoading = true;
            StatusMessage = "Создание групповой заявки...";

            try
            {
                // Сначала сохраняем основного посетителя (если нужно)
                // Для групповой заявки основной посетитель - это текущий пользователь

                var result = await _requestService.CreateGroupRequest(RequestModel, App.CurrentUser.Id, null);

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

        private void GoToMain()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Current.Windows[0]?.Close();
        }

        private void SwitchToIndividual()
        {
            var requestWindow = new RequestWindow();
            requestWindow.Show();
            Application.Current.Windows[0]?.Close();
        }
    }
}
