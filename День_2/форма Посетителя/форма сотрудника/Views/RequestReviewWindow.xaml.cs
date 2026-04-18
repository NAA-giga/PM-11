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

namespace форма_сотрудника.ViewModels
{
    /// <summary>
    /// Логика взаимодействия для RequestReviewWindow.xaml
    /// </summary>
    public partial class RequestReviewWindow : Window
    {
        public RequestReviewWindow(int requestId)
        {
            InitializeComponent();

            var requestService = App.ServiceProvider?.GetRequiredService<IRequestService>();
            var blackListService = App.ServiceProvider?.GetRequiredService<IBlackListService>();

            if (requestService == null || blackListService == null)
            {
                MessageBox.Show("Ошибка инициализации сервисов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            DataContext = new RequestReviewViewModel(requestService, blackListService, requestId);
        }
    }
}
