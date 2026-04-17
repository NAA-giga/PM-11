using System;
using System.Collections.Generic;

namespace форма_Посетителя.Models;

public partial class Заявка
{
    public int Id { get; set; }

    public string? Наименование { get; set; }

    public string? ТипЗаявки { get; set; }

    public DateTime? ДатаСозданиеЗаявки { get; set; }

    public DateTime? ДатаОдобрениеЗаявки { get; set; }

    public string? Цель { get; set; }

    public string? Статус { get; set; }

    public DateOnly ДатаНачалоРеализацииЗаявки { get; set; }

    public DateOnly ДатаОкончаниеРеализацииЗаявки { get; set; }

    public int? ПосетительId { get; set; }

    public int? СотрудникId { get; set; }

    public int? ПодразделениеId { get; set; }

    public virtual Подразделение? Подразделение { get; set; }

    public virtual Посетитель? Посетитель { get; set; }

    public virtual Сотрудник? Сотрудник { get; set; }
}
