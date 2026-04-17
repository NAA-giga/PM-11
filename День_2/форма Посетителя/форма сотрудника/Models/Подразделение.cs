using System;
using System.Collections.Generic;
using System.Text;

namespace форма_сотрудника.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<Заявка> Requests { get; set; } = new List<Заявка>();
    }
}
