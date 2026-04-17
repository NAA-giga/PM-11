using System;
using System.Collections.Generic;
using System.Text;

namespace форма_Посетителя.Models
{
    public class GroupVisitorModel
    {
        public int RowNumber { get; set; }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                UpdateFullName();
                Validate();
            }
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                UpdateFullName();
                Validate();
            }
        }

        private string _middleName = string.Empty;
        public string MiddleName
        {
            get => _middleName;
            set
            {
                _middleName = value;
                UpdateFullName();
            }
        }

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
            }
        }

        public string Phone { get; set; } = string.Empty;

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                Validate();
            }
        }

        public string Note { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;

        private void UpdateFullName()
        {
            _fullName = $"{LastName} {FirstName} {MiddleName}".Trim();
        }

        private void Validate()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(LastName))
            {
                HasError = true;
                ErrorMessage += "Фамилия обязательна; ";
            }
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                HasError = true;
                ErrorMessage += "Имя обязательно; ";
            }
            if (string.IsNullOrWhiteSpace(Email))
            {
                HasError = true;
                ErrorMessage += "Email обязателен; ";
            }
        }
    }
}
