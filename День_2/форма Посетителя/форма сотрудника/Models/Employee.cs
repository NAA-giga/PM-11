using System;
using System.Collections.Generic;
using System.Text;

namespace форма_сотрудника.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public string? Note { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PassportSeries { get; set; }
        public string? PassportNumber { get; set; }
        public string? PhotoPath { get; set; }
        public string? PassportScanPath { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department? Department { get; set; }
        public virtual ICollection<Заявка> Requests { get; set; } = new List<Заявка>();
    }
}
