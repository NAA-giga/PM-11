using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using форма_сотрудника.Helpers;
using форма_сотрудника.Models;
using форма_сотрудника.Services;

namespace форма_сотрудника.ViewModels
{
    public class RequestReviewViewModel : BaseViewModel
    {
        private readonly IRequestService _requestService;
        private readonly IBlackListService _blackListService;
        private readonly int _requestId;
        private string _blackListStatus = "Проверка черного списка...";
        private Заявка _request;
        public Заявка Request
        {
            get => _request;
            set => SetProperty(ref _request, value);
        }
        public string BlackListStatus
        {
            get => _blackListStatus;
            set => SetProperty(ref _blackListStatus, value);
        }
        private DateTime _visitDate = DateTime.Now.AddDays(1);
        public DateTime VisitDate
        {
            get => _visitDate;
            set => SetProperty(ref _visitDate, value);
        }

        private string _visitTime = "10:00";
        public string VisitTime
        {
            get => _visitTime;
            set => SetProperty(ref _visitTime, value);
        }

        private bool _isEditable = true;
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }

        private bool _canApprove = true;
        public bool CanApprove
        {
            get => _canApprove;
            set => SetProperty(ref _canApprove, value);
        }

        private bool _canReject = true;
        public bool CanReject
        {
            get => _canReject;
            set => SetProperty(ref _canReject, value);
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

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand CloseCommand { get; }

        public RequestReviewViewModel(IRequestService requestService, IBlackListService blackListService, int requestId)
        {
            _requestService = requestService;
            _blackListService = blackListService;
            _requestId = requestId;

            ApproveCommand = new RelayCommand(async _ => await Approve(), _ => CanApprove);
            RejectCommand = new RelayCommand(async _ => await Reject(), _ => CanReject);
            CloseCommand = new RelayCommand(_ => Close());

            LoadRequestAndCheckBlackList();
        }

        private async void LoadRequestAndCheckBlackList()
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

                await CheckBlackList();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CheckBlackList()
        {
            IsLoading = true;
            try
            {
                if (Request.VisitorId == null)
                {
                    BlackListStatus = "Ошибка: идентификатор посетителя не найден";
                    return;
                }

                bool isInBlackList = await _blackListService.IsInBlackList(Request.VisitorId.Value);

                if (isInBlackList)
                {
                    BlackListStatus = "⚠️ Посетитель находится в ЧЕРНОМ СПИСКЕ! Заявка автоматически отклонена.";
                    IsEditable = false;
                    CanApprove = false;
                    CanReject = false;

                    await _requestService.RejectRequest(_requestId, App.CurrentEmployee.Id,
                        "Заявитель находится в черном списке");

                    Request.Status = "не одобрена";

                    if (Request.Visitor != null && !string.IsNullOrEmpty(Request.Visitor.Email))
                    {
                        await NotificationHelper.SendNotification(
                            Request.Visitor.Email,
                            "Заявка отклонена",
                            "Заявка на посещение объекта КИИ отклонена в связи с нарушением Федерального закона от 26.07.2017 № 187-ФЗ"
                        );
                    }
                }
                else
                {
                    BlackListStatus = "✅ Посетитель не найден в черном списке.";
                }
            }
            catch (Exception ex)
            {
                BlackListStatus = $"Ошибка проверки: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task Approve()
        {
            if (Request == null) return;

            if (string.IsNullOrWhiteSpace(VisitTime))
            {
                MessageBox.Show("Укажите время посещения!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Одобрить заявку #{Request.Id}?\n\nДата посещения: {VisitDate:dd.MM.yyyy}\nВремя: {VisitTime}",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true;
                try
                {
                    Request.Status = "одобрена";
                    Request.StartDate = DateOnly.FromDateTime(VisitDate);

                    await _requestService.ApproveRequest(_requestId, App.CurrentEmployee.Id);

                    if (Request.Visitor != null && !string.IsNullOrEmpty(Request.Visitor.Email))
                    {
                        await NotificationHelper.SendNotification(
                            Request.Visitor.Email,
                            "Заявка одобрена",
                            $"Заявка на посещение объекта КИИ одобрена, дата посещения: {VisitDate:dd.MM.yyyy}, время посещения: {VisitTime}"
                        );
                    }

                    MessageBox.Show("Заявка успешно одобрена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                    MessageBox.Show($"Ошибка при одобрении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private async Task Reject()
        {
            if (Request == null) return;

            var result = MessageBox.Show($"Отклонить заявку #{Request.Id}?\n\nПричина: указание недостоверных данных",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true;
                try
                {
                    string rejectReason = "указание заявителем заведомо недостоверных данных";

                    await _requestService.RejectRequest(_requestId, App.CurrentEmployee.Id, rejectReason);

                    if (Request.Visitor != null && !string.IsNullOrEmpty(Request.Visitor.Email))
                    {
                        await NotificationHelper.SendNotification(
                            Request.Visitor.Email,
                            "Заявка отклонена",
                            $"Заявка на посещение объекта КИИ отклонена в связи с нарушением Федерального закона от 26.07.2017 № 194-ФЗ по причине {rejectReason}"
                        );
                    }

                    if (Request.VisitorId != null)
                    {
                        await _blackListService.CheckAndAddToBlackList(Request.VisitorId.Value);
                    }

                    MessageBox.Show("Заявка отклонена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                    MessageBox.Show($"Ошибка при отклонении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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
