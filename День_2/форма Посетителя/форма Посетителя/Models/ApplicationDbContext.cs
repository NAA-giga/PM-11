using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace форма_Посетителя.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Заявка> Заявкаs { get; set; }

    public virtual DbSet<Подразделение> Подразделениеs { get; set; }

    public virtual DbSet<Посетитель> Посетительs { get; set; }

    public virtual DbSet<Сотрудник> Сотрудникs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Заявка>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Заявка_pkey");

            entity.ToTable("Заявка");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ДатаНачалоРеализацииЗаявки).HasColumnName("Дата_начало_реализации_заявки");
            entity.Property(e => e.ДатаОдобрениеЗаявки)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Дата_одобрение_заявки");
            entity.Property(e => e.ДатаОкончаниеРеализацииЗаявки).HasColumnName("Дата_окончание_реализации_заявки");
            entity.Property(e => e.ДатаСозданиеЗаявки)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Дата_создание_заявки");
            entity.Property(e => e.Наименование).HasMaxLength(255);
            entity.Property(e => e.ПодразделениеId).HasColumnName("Подразделение_id");
            entity.Property(e => e.ПосетительId).HasColumnName("Посетитель_id");
            entity.Property(e => e.СотрудникId).HasColumnName("Сотрудник_id");
            entity.Property(e => e.Статус)
                .HasMaxLength(20)
                .HasDefaultValueSql("'проверка'::character varying");
            entity.Property(e => e.ТипЗаявки)
                .HasMaxLength(20)
                .HasColumnName("тип_заявки");

            entity.HasOne(d => d.Подразделение).WithMany(p => p.Заявкаs)
                .HasForeignKey(d => d.ПодразделениеId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Заявка_Подразделение_id_fkey");

            entity.HasOne(d => d.Посетитель).WithMany(p => p.Заявкаs)
                .HasForeignKey(d => d.ПосетительId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Заявка_Посетитель_id_fkey");

            entity.HasOne(d => d.Сотрудник).WithMany(p => p.Заявкаs)
                .HasForeignKey(d => d.СотрудникId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Заявка_Сотрудник_id_fkey");
        });

        modelBuilder.Entity<Подразделение>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Подразделение_pkey");

            entity.ToTable("Подразделение");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Наименование).HasMaxLength(255);
        });

        modelBuilder.Entity<Посетитель>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Посетитель_pkey");

            entity.ToTable("Посетитель");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.ДатаРождения).HasColumnName("Дата_рождения");
            entity.Property(e => e.Имя).HasMaxLength(100);
            entity.Property(e => e.НомерПаспорта)
                .HasMaxLength(6)
                .HasColumnName("Номер_паспорта");
            entity.Property(e => e.Организация).HasMaxLength(255);
            entity.Property(e => e.Отчество).HasMaxLength(100);
            entity.Property(e => e.ПодразделениеId).HasColumnName("Подразделение_id");
            entity.Property(e => e.СерияПаспорта)
                .HasMaxLength(4)
                .HasColumnName("Серия_паспорта");
            entity.Property(e => e.СканПаспортаПосетителя)
                .HasMaxLength(500)
                .HasColumnName("скан_паспорта_посетителя");
            entity.Property(e => e.СотрудникId).HasColumnName("Сотрудник_id");
            entity.Property(e => e.Телефон).HasMaxLength(20);
            entity.Property(e => e.Фамилия).HasMaxLength(100);
            entity.Property(e => e.ФотоПосетителя)
                .HasMaxLength(500)
                .HasColumnName("Фото_Посетителя");

            entity.HasOne(d => d.Подразделение).WithMany(p => p.Посетительs)
                .HasForeignKey(d => d.ПодразделениеId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Посетитель_Подразделение_id_fkey");

            entity.HasOne(d => d.Сотрудник).WithMany(p => p.Посетительs)
                .HasForeignKey(d => d.СотрудникId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Посетитель_Сотрудник_id_fkey");
        });

        modelBuilder.Entity<Сотрудник>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Сотрудник_pkey");

            entity.ToTable("Сотрудник");

            entity.HasIndex(e => e.Логин, "Сотрудник_Логин_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.ДатаРождения).HasColumnName("Дата_рождения");
            entity.Property(e => e.Имя).HasMaxLength(100);
            entity.Property(e => e.Логин).HasMaxLength(100);
            entity.Property(e => e.НомерПаспорта)
                .HasMaxLength(6)
                .HasColumnName("Номер_паспорта");
            entity.Property(e => e.Отчество).HasMaxLength(100);
            entity.Property(e => e.Пароль).HasMaxLength(255);
            entity.Property(e => e.ПодразделениеId).HasColumnName("Подразделение_id");
            entity.Property(e => e.СерияПаспорта)
                .HasMaxLength(4)
                .HasColumnName("Серия_паспорта");
            entity.Property(e => e.СканПаспортаПосетителя)
                .HasMaxLength(500)
                .HasColumnName("скан_паспорта_посетителя");
            entity.Property(e => e.Телефон).HasMaxLength(20);
            entity.Property(e => e.Фамилия).HasMaxLength(100);
            entity.Property(e => e.ФотоПосетителя)
                .HasMaxLength(500)
                .HasColumnName("Фото_Посетителя");

            entity.HasOne(d => d.Подразделение).WithMany(p => p.Сотрудникs)
                .HasForeignKey(d => d.ПодразделениеId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Сотрудник_Подразделение_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
