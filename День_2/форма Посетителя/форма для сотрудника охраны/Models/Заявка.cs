using System;
using System.Collections.Generic;
using System.Text;

namespace форма_для_сотрудника_охраны.Models
{
    public class Заявка
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? RequestType { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? Purpose { get; set; }
        public string? Status { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int? VisitorId { get; set; }
        public int? EmployeeId { get; set; }
        public int? DepartmentId { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        public virtual Посетитель? Visitor { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual Department? Department { get; set; }
    }
}
