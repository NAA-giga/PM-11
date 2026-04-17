using System.Windows;
using System.Windows.Input;
using форма_Посетителя.Helpers;
using форма_Посетителя.Views;
namespace форма_Посетителя.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public ICommand NewRequestCommand { get; }
        public ICommand GroupRequestCommand { get; }
        public ICommand MyRequestsCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            var user = App.CurrentUser;
            if (user != null)
            {
                WelcomeMessage = $"Здравствуйте, {user.Имя} {user.Фамилия}!";
            }
            else
            {
                WelcomeMessage = "Здравствуйте!";
            }

            NewRequestCommand = new RelayCommand(_ => OpenNewRequest());
            GroupRequestCommand = new RelayCommand(_ => OpenGroupRequest());
            MyRequestsCommand = new RelayCommand(_ => OpenMyRequests());
            LogoutCommand = new RelayCommand(_ => Logout());
        }

        private void OpenNewRequest()
        {
            var requestWindow = new RequestWindow();
            requestWindow.Show();
            Application.Current.Windows[0]?.Close();
        }

        private void OpenGroupRequest()
        {
            var groupRequestWindow = new GroupRequestWindow();
            groupRequestWindow.Show();
            Application.Current.Windows[0]?.Close();
        }

        private void OpenMyRequests()
        {
            var requestsListWindow = new RequestsListWindow();
            requestsListWindow.Show();
            Application.Current.Windows[0]?.Close();
        }

        private void Logout()
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?",
                "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                App.CurrentUser = null;
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Application.Current.Windows[0]?.Close();
            }
        }
    }
}
