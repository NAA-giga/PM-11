using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System;
using System.Configuration;
using System.IO;
using System.Windows;
using форма_Посетителя.Models;
using форма_Посетителя.Services;
using форма_Посетителя.ViewModels;
using форма_Посетителя.Views;

namespace форма_Посетителя
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;
        public static IServiceProvider? ServiceProvider { get; private set; }
        public static Посетитель? CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Установка лицензии для EPPlus
            ExcelPackage.License.SetNonCommercialPersonal("Student");

            base.OnStartup(e);

            try
            {
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Строка подключения не найдена в App.config!",
                        "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }

                var services = new ServiceCollection();

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString),
                    ServiceLifetime.Scoped);

                services.AddScoped<IRegistrationService, RegistrationService>();
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IRequestService, RequestService>();
                services.AddScoped<IExcelService, ExcelService>();

                services.AddTransient<LoginViewModel>();
                services.AddTransient<RegisterViewModel>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<RequestViewModel>();
                services.AddTransient<GroupRequestViewModel>();
                services.AddTransient<RequestsListViewModel>();
                services.AddTransient<RequestDetailsViewModel>();

                services.AddTransient<LoginWindow>();
                services.AddTransient<RegisterWindow>();
                services.AddTransient<MainWindow>();
                services.AddTransient<RequestWindow>();
                services.AddTransient<GroupRequestWindow>();
                services.AddTransient<RequestsListWindow>();
                services.AddTransient<RequestDetailsWindow>();

                _serviceProvider = services.BuildServiceProvider();
                ServiceProvider = _serviceProvider;

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

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
