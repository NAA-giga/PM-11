using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace форма_Посетителя.Models
{
    public class RequestModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private string _purpose = string.Empty;
        public string Purpose
        {
            get => _purpose;
            set => SetProperty(ref _purpose, value);
        }

        private int? _departmentId;
        public int? DepartmentId
        {
            get => _departmentId;
            set => SetProperty(ref _departmentId, value);
        }

        private int? _employeeId;
        public int? EmployeeId
        {
            get => _employeeId;
            set => SetProperty(ref _employeeId, value);
        }

        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string Note { get; set; } = string.Empty;
        public string PassportSeries { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string EmployeeFullName { get; set; } = string.Empty;
    }
}
