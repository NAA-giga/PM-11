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
using Microsoft.Extensions.DependencyInjection;
using форма_для_сотрудника_охраны.Services;
using форма_для_сотрудника_охраны.ViewModels;

namespace форма_для_сотрудника_охраны.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
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

            var viewModel = new LoginViewModel(authService);
            DataContext = viewModel;

            PasswordBox.PasswordChanged += (s, e) => viewModel.Password = PasswordBox.Password;
        }
    }
}
