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
using форма_для_сотрудника_охраны.Services;
using форма_для_сотрудника_охраны.ViewModels;

namespace форма_для_сотрудника_охраны.Views
{
    /// <summary>
    /// Логика взаимодействия для AccessControlWindow.xaml
    /// </summary>
    public partial class AccessControlWindow : Window
    {
        public AccessControlWindow(int requestId)
        {
            InitializeComponent();

            var requestService = App.ServiceProvider?.GetRequiredService<IRequestService>();
            if (requestService == null)
            {
                MessageBox.Show("Ошибка инициализации сервисов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            DataContext = new AccessControlViewModel(requestService, requestId);

            // Устанавливаем владельца окна
            Owner = Application.Current.MainWindow;
        }

        // Переопределяем закрытие - только текущее окно
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
        }
    }
}
