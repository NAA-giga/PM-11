using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using форма_Посетителя.Models;
using форма_Посетителя.Services;
using форма_Посетителя.Helpers;

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

        private string _selectedMethod;
        public string SelectedMethod
        {
            get => _selectedMethod;
            set => SetProperty(ref _selectedMethod, value);
        }

        public ObservableCollection<string> RegistrationMethods { get; set; }

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

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public RegisterViewModel(IRegistrationService registrationService)
        {
            _registrationService = registrationService;

            RegisterModel = new RegisterModel
            {
                BirthDate = DateTime.Now.AddYears(-20)
            };

            RegistrationMethods = new ObservableCollection<string>
            {
                "SQL запрос (ADO.NET)",
                "Хранимая процедура",
                "Entity Framework Core (ORM)"
            };
            SelectedMethod = RegistrationMethods[0];

            RegisterCommand = new RelayCommand(async _ => await Register(), _ => CanRegister());
            GoToLoginCommand = new RelayCommand(_ => GoToLogin());
        }

        private bool CanRegister()
        {
            return !IsLoading && RegisterModel != null;
        }

        private async Task Register()
        {
            IsLoading = true;
            StatusMessage = "Регистрация...";

            try
            {
                (bool success, string message, int visitorId) result;

                switch (SelectedMethod)
                {
                    case "SQL запрос (ADO.NET)":
                        result = await _registrationService.RegisterWithSQL(RegisterModel);
                        break;
                    case "Хранимая процедура":
                        result = await _registrationService.RegisterWithStoredProcedure(RegisterModel);
                        break;
                    case "Entity Framework Core (ORM)":
                        result = await _registrationService.RegisterWithEF(RegisterModel);
                        break;
                    default:
                        result = (false, "Неизвестный способ регистрации", 0);
                        break;
                }

                if (result.success)
                {
                    StatusMessage = result.message;
                    MessageBox.Show(result.message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    GoToLogin();
                }
                else
                {
                    StatusMessage = result.message;
                    MessageBox.Show(result.message, "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
            Application.Current.Windows[0]?.Close();
        }
    }
}
