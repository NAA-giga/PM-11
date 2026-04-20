using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using форма_для_сотрудника_охраны.Helpers;
using форма_для_сотрудника_охраны.Models;
using форма_для_сотрудника_охраны.Services;
using форма_для_сотрудника_охраны.Views;

namespace форма_для_сотрудника_охраны.ViewModels
{
    public class AccessControlViewModel : BaseViewModel
    {
        private readonly IRequestService _requestService;
        private readonly int _requestId;

        private Заявка _request;
        public Заявка Request
        {
            get => _request;
            set => SetProperty(ref _request, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _hasEntry = false;
        public bool HasEntry
        {
            get => _hasEntry;
            set => SetProperty(ref _hasEntry, value);
        }

        private bool _hasExit = false;
        public bool HasExit
        {
            get => _hasExit;
            set => SetProperty(ref _hasExit, value);
        }

        private string _entryTime = string.Empty;
        public string EntryTime
        {
            get => _entryTime;
            set => SetProperty(ref _entryTime, value);
        }

        private string _exitTime = string.Empty;
        public string ExitTime
        {
            get => _exitTime;
            set => SetProperty(ref _exitTime, value);
        }

        public ICommand GrantAccessCommand { get; }
        public ICommand RecordExitCommand { get; }
        public ICommand CloseCommand { get; }

        public AccessControlViewModel(IRequestService requestService, int requestId)
        {
            _requestService = requestService;
            _requestId = requestId;

            GrantAccessCommand = new RelayCommand(async _ => await GrantAccess(), _ => !HasEntry && !IsLoading);
            RecordExitCommand = new RelayCommand(async _ => await RecordExit(), _ => HasEntry && !HasExit && !IsLoading);
            CloseCommand = new RelayCommand(_ => CloseWindow());

            LoadRequestDetails();
        }

        private async void LoadRequestDetails()
        {
            IsLoading = true;
            StatusMessage = "Загрузка данных...";

            try
            {
                Request = await _requestService.GetRequestDetails(_requestId);

                if (Request == null)
                {
                    StatusMessage = "Заявка не найдена";
                    return;
                }

                if (Request.EntryTime.HasValue)
                {
                    HasEntry = true;
                    EntryTime = Request.EntryTime.Value.ToString("dd.MM.yyyy HH:mm:ss");
                }

                if (Request.ExitTime.HasValue)
                {
                    HasExit = true;
                    ExitTime = Request.ExitTime.Value.ToString("dd.MM.yyyy HH:mm:ss");
                }

                StatusMessage = "Данные загружены";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GrantAccess()
        {
            if (Request == null) return;

            var result = MessageBox.Show($"Предоставить доступ для {Request.Visitor?.LastName} {Request.Visitor?.FirstName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true;
                try
                {
                    var response = await _requestService.GrantAccess(Request.Id, App.CurrentEmployee.Id);

                    if (response.Success)
                    {
                        StatusMessage = response.Message;
                        HasEntry = true;
                        EntryTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                        MessageBox.Show(response.Message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = response.Message;
                        MessageBox.Show(response.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private async Task RecordExit()
        {
            if (Request == null) return;

            var result = MessageBox.Show($"Зафиксировать убытие {Request.Visitor?.LastName} {Request.Visitor?.FirstName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true;
                try
                {
                    var response = await _requestService.RecordExit(Request.Id, App.CurrentEmployee.Id);

                    if (response.Success)
                    {
                        StatusMessage = response.Message;
                        HasExit = true;
                        ExitTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                        MessageBox.Show(response.Message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = response.Message;
                        MessageBox.Show(response.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private void CloseWindow()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is AccessControlWindow);
            currentWindow?.Close();
        }
    }
}
