using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using форма_для_сотрудника_охраны.Helpers;
using форма_для_сотрудника_охраны.Views;

namespace форма_для_сотрудника_охраны.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _welcomeMessage = string.Empty;
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        private string _department = string.Empty;
        public string Department
        {
            get => _department;
            set => SetProperty(ref _department, value);
        }

        public ICommand ViewRequestsCommand { get; }
        public ICommand SearchRequestsCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            var employee = App.CurrentEmployee;
            if (employee != null)
            {
                FullName = $"{employee.LastName} {employee.FirstName} {employee.MiddleName}".Trim();
                Department = employee.Department?.Name ?? "Отдел пропускного режима";
                WelcomeMessage = $"Здравствуйте, {employee.FirstName} {employee.LastName}!";
            }
            else
            {
                WelcomeMessage = "Здравствуйте!";
            }

            ViewRequestsCommand = new RelayCommand(_ => OpenRequestsList());
            SearchRequestsCommand = new RelayCommand(_ => OpenSearch());
            LogoutCommand = new RelayCommand(_ => Logout());
        }

        private void OpenRequestsList()
        {
            var requestsWindow = new RequestsListWindow();
            requestsWindow.Show();
            // НЕ закрываем MainWindow
            // Application.Current.Windows[0]?.Close();
        }

        private void OpenSearch()
        {
            var searchWindow = new SearchWindow();
            searchWindow.Show();
            // НЕ закрываем MainWindow
            // Application.Current.Windows[0]?.Close();
        }

        private void Logout()
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?",
                "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                App.CurrentEmployee = null;
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Application.Current.Windows[0]?.Close();
            }
        }
    }
}
