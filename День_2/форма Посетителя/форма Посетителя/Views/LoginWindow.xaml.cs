using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using форма_Посетителя.Services;
using форма_Посетителя.ViewModels;
using форма_Посетителя.Helpers;

namespace форма_Посетителя.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();

            var authService = App.ServiceProvider?.GetRequiredService<IAuthService>();
            if (authService == null)
            {
                MessageBox.Show("Ошибка инициализации сервисов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            _viewModel = new LoginViewModel(authService);
            DataContext = _viewModel;

            // Обновляем пароль при его изменении
            PasswordBox.PasswordChanged += (s, e) =>
            {
                _viewModel.Password = PasswordBox.Password;
            };
        }
    }
}
