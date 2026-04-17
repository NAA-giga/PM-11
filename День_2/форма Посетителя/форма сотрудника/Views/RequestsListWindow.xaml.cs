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
using форма_сотрудника.Services;
using форма_сотрудника.ViewModels;

namespace форма_сотрудника.Views
{
    /// <summary>
    /// Логика взаимодействия для RequestsListWindow.xaml
    /// </summary>
    public partial class RequestsListWindow : Window
    {
        public RequestsListWindow(bool isMyRequests)
        {
            InitializeComponent();

            var requestService = App.ServiceProvider?.GetRequiredService<IRequestService>();
            if (requestService == null)
            {
                MessageBox.Show("Ошибка инициализации сервисов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            DataContext = new RequestsListViewModel(requestService, isMyRequests);
        }
    }
}
