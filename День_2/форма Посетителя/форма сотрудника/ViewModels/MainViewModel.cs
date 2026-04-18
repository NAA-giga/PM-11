using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using форма_сотрудника.Helpers;
using форма_сотрудника.Views;

namespace форма_сотрудника.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand DepartmentRequestsCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            DepartmentRequestsCommand = new RelayCommand(_ => OpenDepartmentRequests());
            LogoutCommand = new RelayCommand(_ => Logout());
        }

        private void OpenDepartmentRequests()
        {
            // Убираем параметр isMyRequests
            var requestsWindow = new RequestsListWindow();
            requestsWindow.Show();
            Application.Current.Windows[0]?.Close();
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
