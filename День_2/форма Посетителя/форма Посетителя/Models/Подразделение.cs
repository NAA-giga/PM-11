using System;
using System.Collections.Generic;

namespace форма_Посетителя.Models;

public partial class Подразделение
{
    public int Id { get; set; }

    public string Наименование { get; set; } = null!;

    public virtual ICollection<Заявка> Заявкаs { get; set; } = new List<Заявка>();

    public virtual ICollection<Посетитель> Посетительs { get; set; } = new List<Посетитель>();

    public virtual ICollection<Сотрудник> Сотрудникs { get; set; } = new List<Сотрудник>();
}
