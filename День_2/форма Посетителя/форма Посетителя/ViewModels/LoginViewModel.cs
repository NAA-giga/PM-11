using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
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
        public ICommand GoToRegisterCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(async _ => await Login(), _ => CanLogin());
            GoToRegisterCommand = new RelayCommand(_ => GoToRegister());
        }

        private bool CanLogin()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private async Task Login()
        {
            IsLoading = true;
            StatusMessage = "Выполняется вход...";

            try
            {
                var result = await _authService.LoginWithEF(Email, Password);

                if (result.Success && result.Visitor != null)
                {
                    StatusMessage = "Вход выполнен успешно!";
                    App.CurrentUser = result.Visitor;

                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Application.Current.Windows[0]?.Close();
                }
                else
                {
                    StatusMessage = "Неверный email или пароль!";
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

        private void GoToRegister()
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            Application.Current.Windows[0]?.Close();
        }
    }
}
