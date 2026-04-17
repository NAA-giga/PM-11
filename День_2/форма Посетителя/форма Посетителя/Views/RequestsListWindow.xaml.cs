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

namespace форма_Посетителя.Views
{
    /// <summary>
    /// Логика взаимодействия для RequestsListWindow.xaml
    /// </summary>
    public partial class RequestsListWindow : Window
    {
        public RequestsListWindow()
        {
            InitializeComponent();

            var requestService = App.ServiceProvider.GetRequiredService<IRequestService>();
            DataContext = new RequestsListViewModel(requestService);
        }
    }
}
