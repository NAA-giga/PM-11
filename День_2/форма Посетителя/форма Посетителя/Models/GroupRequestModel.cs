using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace форма_Посетителя.Models
{
    public class GroupRequestModel
    {
        // Информация для пропуска
        [Required(ErrorMessage = "Дата начала обязательна")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Дата окончания обязательна")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Цель посещения обязательна")]
        [MinLength(5, ErrorMessage = "Цель должна содержать минимум 5 символов")]
        public string Purpose { get; set; }

        // Принимающая сторона
        [Required(ErrorMessage = "Подразделение обязательно")]
        public int? DepartmentId { get; set; }

        [Required(ErrorMessage = "Сотрудник обязателен")]
        public int? EmployeeId { get; set; }

        // Информация о посетителе (основной)
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Note { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string Organization { get; set; }

        // Список посетителей для групповой заявки
        public List<GroupVisitorModel> Visitors { get; set; } = new List<GroupVisitorModel>();

        // Для отображения
        public string DepartmentName { get; set; }
        public string EmployeeFullName { get; set; }
    }

}
