using System;
using System.Collections.Generic;
using System.Text;

namespace форма_для_сотрудника_охраны.Models
{
    public class Посетитель
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Note { get; set; }
        public string? PassportSeries { get; set; }
        public string? PassportNumber { get; set; }
        public string? Organization { get; set; }

        public virtual ICollection<Заявка> Requests { get; set; } = new List<Заявка>();
    }
}
