using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Input;
using форма_Посетителя.Models;
using форма_Посетителя.Services;
using форма_Посетителя.Views;
using форма_Посетителя.Helpers;

namespace форма_Посетителя.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        private string _loginText = string.Empty;
        public string LoginText
        {
            get => _loginText;
            set => SetProperty(ref _loginText, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
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

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;

            LoginCommand = new RelayCommand(async _ => await Login(), _ => CanLogin());
            GoToRegisterCommand = new RelayCommand(_ => GoToRegister());
        }

        private bool CanLogin()
        {
            return !IsLoading && !string.IsNullOrWhiteSpace(LoginText) && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task Login()
        {
            IsLoading = true;
            StatusMessage = "Выполняется вход...";

            try
            {
                var result = await _authService.LoginWithEF(LoginText, Password);

                if (result.Success && result.Visitor != null)
                {
                    StatusMessage = result.Message;

                    App.CurrentUser = result.Visitor;

                    MessageBox.Show($"Добро пожаловать, {result.Visitor.Имя} {result.Visitor.Фамилия}!",
                        "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);

                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    Application.Current.Windows[0]?.Close();
                }
                else
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void GoToRegister()
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            Application.Current.Windows[0]?.Close();
        }
    }
}
