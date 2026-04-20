using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using форма_для_сотрудника_охраны.Helpers;
using форма_для_сотрудника_охраны.Models;
using форма_для_сотрудника_охраны.Services;
using форма_для_сотрудника_охраны.Views;

namespace форма_для_сотрудника_охраны.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private readonly IRequestService _requestService;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private ObservableCollection<Заявка> _searchResults = new ObservableCollection<Заявка>();
        public ObservableCollection<Заявка> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        private Заявка? _selectedRequest;
        public Заявка? SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                SetProperty(ref _selectedRequest, value);
                (OpenAccessCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DepartureCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
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

        public ICommand SearchCommand { get; }
        public ICommand OpenAccessCommand { get; }
        public ICommand DepartureCommand { get; }
        public ICommand CloseCommand { get; }

        public SearchViewModel(IRequestService requestService)
        {
            _requestService = requestService;

            SearchCommand = new RelayCommand(async _ => await Search(), _ => CanSearch());
            OpenAccessCommand = new RelayCommand(_ => OpenAccess(), _ => SelectedRequest != null);
            DepartureCommand = new RelayCommand(_ => OpenDeparture(), _ => SelectedRequest != null);
            CloseCommand = new RelayCommand(_ => CloseWindow());
        }

        private bool CanSearch()
        {
            return !IsLoading && !string.IsNullOrWhiteSpace(SearchText);
        }

        private async Task Search()
        {
            IsLoading = true;
            StatusMessage = "Поиск...";
            SearchResults.Clear();

            try
            {
                var results = await _requestService.SearchRequests(SearchText);

                foreach (var request in results)
                {
                    SearchResults.Add(request);
                }

                StatusMessage = results.Count > 0
                    ? $"Найдено заявок: {results.Count}"
                    : "Заявки не найдены. Попробуйте изменить поисковый запрос.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка поиска: {ex.Message}";
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OpenAccess()
        {
            if (SelectedRequest != null)
            {
                var accessWindow = new AccessControlWindow(SelectedRequest.Id);
                accessWindow.ShowDialog();
                _ = Search();
            }
        }

        private void OpenDeparture()
        {
            if (SelectedRequest != null)
            {
                var departureWindow = new DepartureWindow(SelectedRequest.Id);
                departureWindow.ShowDialog();
                _ = Search();
            }
        }

        private void CloseWindow()
        {
            // Находим и закрываем только окно поиска
            foreach (Window window in Application.Current.Windows)
            {
                if (window is SearchWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
