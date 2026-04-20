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
using форма_Посетителя.Services;
using форма_Посетителя.ViewModels;

namespace форма_Посетителя.Views
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            var registrationService = App.ServiceProvider?.GetRequiredService<IRegistrationService>();
            if (registrationService == null)
            {
                MessageBox.Show("Ошибка инициализации сервисов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            var viewModel = new RegisterViewModel(registrationService);
            DataContext = viewModel;

            PasswordBox.PasswordChanged += (s, e) => viewModel.RegisterModel.Password = PasswordBox.Password;
            ConfirmPasswordBox.PasswordChanged += (s, e) => viewModel.RegisterModel.ConfirmPassword = ConfirmPasswordBox.Password;
        }
    }
}
