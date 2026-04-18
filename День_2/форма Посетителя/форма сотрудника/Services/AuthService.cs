using System;
using System.Collections.Generic;
using System.Text;
using форма_сотрудника.Data;
using форма_сотрудника.Models;
using Microsoft.EntityFrameworkCore;
using форма_сотрудника.Helpers;

namespace форма_сотрудника.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _connectionString;

        public AuthService(ApplicationDbContext context)
        {
            _connectionString = context.Database.GetConnectionString();
        }

        private ApplicationDbContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(_connectionString);
            return new ApplicationDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// Вход по email и паролю (для посетителей)
        /// </summary>
        public async Task<(bool Success, string Message, Employee? Employee)> Login(string email, string password)
        {
            using (var context = CreateContext())
            {
                try
                {
                    string hashedPassword = PasswordHasher.HashPasswordMD5(password);

                    var employee = await context.Employees
                        .Include(e => e.Department)
                        .FirstOrDefaultAsync(e => e.Email == email && e.Password == hashedPassword);

                    if (employee != null)
                    {
                        return (true, "Вход выполнен успешно!", employee);
                    }

                    return (false, "Неверный email или пароль!", null);
                }
                catch (System.Exception ex)
                {
                    return (false, $"Ошибка авторизации: {ex.Message}", null);
                }
            }
        }

        /// <summary>
        /// Вход по коду сотрудника (логину) и паролю
        /// </summary>
        public async Task<(bool Success, string Message, Employee? Employee)> LoginByCode(string login, string password)
        {
            using (var context = CreateContext())
            {
                try
                {
                    string hashedPassword = PasswordHasher.HashPasswordMD5(password);

                    var employee = await context.Employees
                        .Include(e => e.Department)
                        .FirstOrDefaultAsync(e => e.Login == login && e.Password == hashedPassword);

                    if (employee != null)
                    {
                        return (true, "Вход выполнен успешно!", employee);
                    }

                    return (false, "Неверный код сотрудника или пароль!", null);
                }
                catch (System.Exception ex)
                {
                    return (false, $"Ошибка авторизации: {ex.Message}", null);
                }
            }
        }
    }
}
