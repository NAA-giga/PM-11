using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using форма_сотрудника.Helpers;
using форма_сотрудника.Services;

namespace форма_сотрудника.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly RelayCommand _loginCommand;

        private string _loginText = string.Empty;
        public string LoginText
        {
            get => _loginText;
            set
            {
                SetProperty(ref _loginText, value);
                _loginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                _loginCommand.RaiseCanExecuteChanged();
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

        public ICommand LoginCommand => _loginCommand;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            _loginCommand = new RelayCommand(async _ => await Login(), _ => CanLogin());
        }

        private bool CanLogin()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(LoginText) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private async Task Login()
        {
            IsLoading = true;
            StatusMessage = "Выполняется вход...";
            _loginCommand.RaiseCanExecuteChanged();

            try
            {
                var result = await _authService.Login(LoginText, Password);

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
                    StatusMessage = "Неверный логин или пароль!";
                }
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                _loginCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
