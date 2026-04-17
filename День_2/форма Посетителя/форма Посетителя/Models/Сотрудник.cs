using System;
using System.Collections.Generic;

namespace форма_Посетителя.Models;

public partial class Сотрудник
{
    public int Id { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string? Отчество { get; set; }

    public string? Телефон { get; set; }

    public string Email { get; set; } = null!;

    public DateOnly? ДатаРождения { get; set; }

    public string? Примечание { get; set; }

    public string Логин { get; set; } = null!;

    public string Пароль { get; set; } = null!;

    public string? СерияПаспорта { get; set; }

    public string? НомерПаспорта { get; set; }

    public string? ФотоПосетителя { get; set; }

    public string? СканПаспортаПосетителя { get; set; }

    public int? ПодразделениеId { get; set; }

    public virtual ICollection<Заявка> Заявкаs { get; set; } = new List<Заявка>();

    public virtual Подразделение? Подразделение { get; set; }

    public virtual ICollection<Посетитель> Посетительs { get; set; } = new List<Посетитель>();
}
