using System;
using System.Collections.Generic;
using System.Text;

namespace форма_сотрудника.Models
{
    public class BlackList
    {
        public int Id { get; set; }
        public int VisitorId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; }

        public virtual Посетитель? Visitor { get; set; }
    }
}
