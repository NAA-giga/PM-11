using System;
using System.Collections.Generic;
using System.Text;

namespace форма_для_сотрудника_охраны.Models
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
        public int? DepartmentId { get; set; }

        public virtual Department? Department { get; set; }
        public virtual ICollection<Заявка> Requests { get; set; } = new List<Заявка>();
    }
}
