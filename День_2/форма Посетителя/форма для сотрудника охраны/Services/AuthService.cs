using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using форма_для_сотрудника_охраны.Data;
using форма_для_сотрудника_охраны.Helpers;
using форма_для_сотрудника_охраны.Models;

namespace форма_для_сотрудника_охраны.Services
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
