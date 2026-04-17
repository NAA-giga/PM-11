using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace форма_Посетителя.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(8, ErrorMessage = "Пароль должен быть не менее 8 символов")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имя обязательно")]
        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        [Phone(ErrorMessage = "Неверный формат телефона")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Дата рождения обязательна")]
        [MinimumAge(16, ErrorMessage = "Возраст должен быть не менее 16 лет")]
        public DateTime BirthDate { get; set; } = DateTime.Now.AddYears(-20);

        [Required(ErrorMessage = "Серия паспорта обязательна")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Серия паспорта должна содержать 4 символа")]
        public string PassportSeries { get; set; } = string.Empty;

        [Required(ErrorMessage = "Номер паспорта обязателен")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Номер паспорта должен содержать 6 символов")]
        public string PassportNumber { get; set; } = string.Empty;

        public string? Organization { get; set; }

        public string? Note { get; set; }
    }

    /// <summary>
    /// Кастомная валидация для возраста
    /// </summary>
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > today.AddYears(-age)) age--;

                if (age < _minimumAge)
                {
                    return new ValidationResult($"Возраст должен быть не менее {_minimumAge} лет");
                }
            }
            return ValidationResult.Success;
        }
    }
}
