using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using форма_сотрудника.Helpers;
using форма_сотрудника.Models;
using форма_сотрудника.Services;

namespace форма_сотрудника.ViewModels
{
    public class RequestDetailsViewModel : BaseViewModel
    {
        private readonly IRequestService _requestService;
        private readonly int _requestId;

        private Заявка _request;
        public Заявка Request
        {
            get => _request;
            set => SetProperty(ref _request, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand CloseCommand { get; }

        public RequestDetailsViewModel(IRequestService requestService, int requestId)
        {
            _requestService = requestService;
            _requestId = requestId;

            CloseCommand = new RelayCommand(_ => Close());

            LoadRequestDetails();
        }

        private async void LoadRequestDetails()
        {
            IsLoading = true;
            StatusMessage = "Загрузка деталей заявки...";

            try
            {
                Request = await _requestService.GetRequestDetails(_requestId);
                StatusMessage = "Детали заявки загружены";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки деталей заявки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Close()
        {
            // Закрываем только текущее окно (детали заявки)
            Application.Current.Windows[0]?.Close();
        }
    }
}
