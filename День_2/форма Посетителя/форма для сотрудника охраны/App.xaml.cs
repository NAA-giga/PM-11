using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Configuration;
using System.Data;
using System.Windows;
using форма_для_сотрудника_охраны.Data;
using форма_для_сотрудника_охраны.Models;
using форма_для_сотрудника_охраны.Services;
using форма_для_сотрудника_охраны.ViewModels;
using форма_для_сотрудника_охраны.Views;

namespace форма_для_сотрудника_охраны
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;
        public static IServiceProvider? ServiceProvider { get; private set; }
        public static Employee? CurrentEmployee { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Строка подключения не найдена в App.config!",
                        "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }

                var services = new ServiceCollection();

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString), ServiceLifetime.Transient);

                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IRequestService, RequestService>();

                services.AddTransient<LoginViewModel>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<RequestsListViewModel>();
                services.AddTransient<AccessControlViewModel>();
                services.AddTransient<DepartureViewModel>();

                services.AddTransient<LoginWindow>();
                services.AddTransient<MainWindow>();
                services.AddTransient<RequestsListWindow>();
                services.AddTransient<AccessControlWindow>();
                services.AddTransient<DepartureWindow>();

                _serviceProvider = services.BuildServiceProvider();
                ServiceProvider = _serviceProvider;

                // Запускаем окно авторизации, а не MainWindow
                var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
