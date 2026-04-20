using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using форма_Посетителя.Helpers;
using форма_Посетителя.Models;
using форма_Посетителя.Services;
using форма_Посетителя.Views;

namespace форма_Посетителя.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IRegistrationService _registrationService;

        private RegisterModel _registerModel;
        public RegisterModel RegisterModel
        {
            get => _registerModel;
            set => SetProperty(ref _registerModel, value);
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

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public RegisterViewModel(IRegistrationService registrationService)
        {
            _registrationService = registrationService;

            RegisterModel = new RegisterModel
            {
                BirthDate = DateTime.Now.AddYears(-20)
            };

            RegisterCommand = new RelayCommand(async _ => await Register(), _ => CanRegister());
            GoToLoginCommand = new RelayCommand(_ => GoToLogin());
        }

        private bool CanRegister()
        {
            return !IsLoading && RegisterModel != null &&
                   !string.IsNullOrWhiteSpace(RegisterModel.Email) &&
                   !string.IsNullOrWhiteSpace(RegisterModel.Password) &&
                   RegisterModel.Password == RegisterModel.ConfirmPassword &&
                   !string.IsNullOrWhiteSpace(RegisterModel.LastName) &&
                   !string.IsNullOrWhiteSpace(RegisterModel.FirstName) &&
                   RegisterModel.BirthDate <= DateTime.Now.AddYears(-16);
        }

        private async Task Register()
        {
            IsLoading = true;
            StatusMessage = "Регистрация...";

            try
            {
                // Используем только ORM способ
                var result = await _registrationService.RegisterWithEF(RegisterModel);

                if (result.Success)
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    GoToLogin();
                }
                else
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void GoToLogin()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Application.Current.Windows[0]?.Close();
        }
    }
}
