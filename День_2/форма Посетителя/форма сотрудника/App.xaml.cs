using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using форма_сотрудника.Data;
using форма_сотрудника.Models;
using форма_сотрудника.Services;
using форма_сотрудника.ViewModels;
using форма_сотрудника.Views;

namespace форма_сотрудника
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
                services.AddScoped<IBlackListService, BlackListService>();

                services.AddTransient<LoginViewModel>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<RequestsListViewModel>();
                services.AddTransient<RequestDetailsViewModel>();

                services.AddTransient<LoginWindow>();
                services.AddTransient<MainWindow>();
                services.AddTransient<RequestsListWindow>();
                services.AddTransient<RequestDetailsWindow>();
                services.AddTransient<InputDialog>();

                _serviceProvider = services.BuildServiceProvider();
                ServiceProvider = _serviceProvider;

                // Запускаем окно авторизации
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
