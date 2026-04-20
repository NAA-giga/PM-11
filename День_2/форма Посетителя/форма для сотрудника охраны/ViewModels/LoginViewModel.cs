using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
using форма_для_сотрудника_охраны.Helpers;
using форма_для_сотрудника_охраны.Services;

namespace форма_для_сотрудника_охраны.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        private string _employeeCode = string.Empty;
        public string EmployeeCode
        {
            get => _employeeCode;
            set
            {
                SetProperty(ref _employeeCode, value);
                (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
            set
            {
                SetProperty(ref _isLoading, value);
                (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(async _ => await Login(), _ => CanLogin());
        }

        private bool CanLogin()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(EmployeeCode) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private async Task Login()
        {
            IsLoading = true;
            StatusMessage = "Выполняется вход...";

            try
            {
                var result = await _authService.LoginByCode(EmployeeCode, Password);

                if (result.Success && result.Employee != null)
                {
                    StatusMessage = "Вход выполнен успешно!";
                    App.CurrentEmployee = result.Employee;

                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Application.Current.Windows[0]?.Close();
                }
                else
                {
                    StatusMessage = "Неверный код сотрудника или пароль!";
                }
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
