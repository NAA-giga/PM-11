using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using форма_для_сотрудника_охраны.Models;

namespace форма_для_сотрудника_охраны.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Посетитель> Visitors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Заявка> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().ToTable("Сотрудник");
            modelBuilder.Entity<Employee>().HasKey(e => e.Id);
            modelBuilder.Entity<Employee>().Property(e => e.Id).HasColumnName("id");
            modelBuilder.Entity<Employee>().Property(e => e.LastName).HasColumnName("Фамилия");
            modelBuilder.Entity<Employee>().Property(e => e.FirstName).HasColumnName("Имя");
            modelBuilder.Entity<Employee>().Property(e => e.MiddleName).HasColumnName("Отчество");
            modelBuilder.Entity<Employee>().Property(e => e.Phone).HasColumnName("Телефон");
            modelBuilder.Entity<Employee>().Property(e => e.Email).HasColumnName("email");
            modelBuilder.Entity<Employee>().Property(e => e.BirthDate).HasColumnName("Дата_рождения");
            modelBuilder.Entity<Employee>().Property(e => e.Note).HasColumnName("Примечание");
            modelBuilder.Entity<Employee>().Property(e => e.Login).HasColumnName("Логин");
            modelBuilder.Entity<Employee>().Property(e => e.Password).HasColumnName("Пароль");
            modelBuilder.Entity<Employee>().Property(e => e.DepartmentId).HasColumnName("Подразделение_id");

            modelBuilder.Entity<Посетитель>().ToTable("Посетитель");
            modelBuilder.Entity<Посетитель>().HasKey(v => v.Id);
            modelBuilder.Entity<Посетитель>().Property(v => v.Id).HasColumnName("id");
            modelBuilder.Entity<Посетитель>().Property(v => v.LastName).HasColumnName("Фамилия");
            modelBuilder.Entity<Посетитель>().Property(v => v.FirstName).HasColumnName("Имя");
            modelBuilder.Entity<Посетитель>().Property(v => v.MiddleName).HasColumnName("Отчество");
            modelBuilder.Entity<Посетитель>().Property(v => v.Phone).HasColumnName("Телефон");
            modelBuilder.Entity<Посетитель>().Property(v => v.Email).HasColumnName("email");
            modelBuilder.Entity<Посетитель>().Property(v => v.BirthDate).HasColumnName("Дата_рождения");
            modelBuilder.Entity<Посетитель>().Property(v => v.Note).HasColumnName("Примечание");
            modelBuilder.Entity<Посетитель>().Property(v => v.PassportSeries).HasColumnName("Серия_паспорта");
            modelBuilder.Entity<Посетитель>().Property(v => v.PassportNumber).HasColumnName("Номер_паспорта");
            modelBuilder.Entity<Посетитель>().Property(v => v.Organization).HasColumnName("Организация");

            modelBuilder.Entity<Department>().ToTable("Подразделение");
            modelBuilder.Entity<Department>().HasKey(d => d.Id);
            modelBuilder.Entity<Department>().Property(d => d.Id).HasColumnName("id");
            modelBuilder.Entity<Department>().Property(d => d.Name).HasColumnName("Наименование");

            modelBuilder.Entity<Заявка>().ToTable("Заявка");
            modelBuilder.Entity<Заявка>().HasKey(r => r.Id);
            modelBuilder.Entity<Заявка>().Property(r => r.Id).HasColumnName("id");
            modelBuilder.Entity<Заявка>().Property(r => r.Name).HasColumnName("Наименование");
            modelBuilder.Entity<Заявка>().Property(r => r.RequestType).HasColumnName("тип_заявки");
            modelBuilder.Entity<Заявка>().Property(r => r.CreationDate).HasColumnName("Дата_создание_заявки");
            modelBuilder.Entity<Заявка>().Property(r => r.ApprovalDate).HasColumnName("Дата_одобрение_заявки");
            modelBuilder.Entity<Заявка>().Property(r => r.Purpose).HasColumnName("Цель");
            modelBuilder.Entity<Заявка>().Property(r => r.Status).HasColumnName("Статус");
            modelBuilder.Entity<Заявка>().Property(r => r.StartDate).HasColumnName("Дата_начало_реализации_заявки");
            modelBuilder.Entity<Заявка>().Property(r => r.EndDate).HasColumnName("Дата_окончание_реализации_заявки");
            modelBuilder.Entity<Заявка>().Property(r => r.VisitorId).HasColumnName("Посетитель_id");
            modelBuilder.Entity<Заявка>().Property(r => r.EmployeeId).HasColumnName("Сотрудник_id");
            modelBuilder.Entity<Заявка>().Property(r => r.DepartmentId).HasColumnName("Подразделение_id");
            modelBuilder.Entity<Заявка>().Property(r => r.RejectionReason).HasColumnName("Причина_отказа");
            modelBuilder.Entity<Заявка>().Property(r => r.EntryTime).HasColumnName("Время_входа");
            modelBuilder.Entity<Заявка>().Property(r => r.ExitTime).HasColumnName("Время_выхода");
        }
    }
}
