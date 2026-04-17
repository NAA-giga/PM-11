using System;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using System.Windows.Input;
using форма_Посетителя.Models;
using форма_Посетителя.Services;
using форма_Посетителя.Helpers;

namespace форма_Посетителя.ViewModels
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

            try
            {
                Request = await _requestService.GetRequestWithDetails(_requestId);
            }
            catch (Exception ex)
            {
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
            Application.Current.Windows[0]?.Close();
        }
    }
}
