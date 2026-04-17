using System;
using System.Collections.Generic;
using System.Text;

namespace форма_Посетителя.Models
{
    public class EmployeeItem
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
