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
    public class DepartureViewModel : BaseViewModel
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

        private bool _canRecordExit = false;
        public bool CanRecordExit
        {
            get => _canRecordExit;
            set => SetProperty(ref _canRecordExit, value);
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

        public ICommand RecordExitCommand { get; }
        public ICommand CloseCommand { get; }

        public DepartureViewModel(IRequestService requestService, int requestId)
        {
            _requestService = requestService;
            _requestId = requestId;

            RecordExitCommand = new RelayCommand(async _ => await RecordExit(), _ => CanRecordExit);
            CloseCommand = new RelayCommand(_ => Close());

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
                    EntryTime = Request.EntryTime.Value.ToString("dd.MM.yyyy HH:mm:ss");

                    if (Request.ExitTime.HasValue)
                    {
                        ExitTime = Request.ExitTime.Value.ToString("dd.MM.yyyy HH:mm:ss");
                        CanRecordExit = false;
                        StatusMessage = "Убытие уже зафиксировано";
                    }
                    else
                    {
                        CanRecordExit = true;
                        StatusMessage = "Вход зафиксирован. Можно зафиксировать убытие.";
                    }
                }
                else
                {
                    CanRecordExit = false;
                    StatusMessage = "Время входа не зафиксировано. Сначала необходимо открыть турникет.";
                }
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

        private async Task RecordExit()
        {
            if (Request == null) return;

            var result = MessageBox.Show(
                $"Зафиксировать убытие?\n\n" +
                $"Посетитель: {Request.Visitor?.LastName} {Request.Visitor?.FirstName}\n" +
                $"Время входа: {EntryTime}\n" +
                $"Текущее время: {DateTime.Now:HH:mm:ss}",
                "Подтверждение убытия",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true;
                try
                {
                    var response = await _requestService.RecordExit(Request.Id, App.CurrentEmployee.Id);

                    if (response.Success)
                    {
                        StatusMessage = response.Message;
                        ExitTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                        CanRecordExit = false;

                        MessageBox.Show(response.Message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        await Task.Delay(500);
                        Close();
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

        private void Close()
        {
            Application.Current.Windows[0]?.Close();
        }
    }
}
