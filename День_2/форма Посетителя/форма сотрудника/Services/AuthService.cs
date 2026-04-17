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
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, Employee? Employee)> Login(string login, string password)
        {
            try
            {
                string hashedPassword = PasswordHasher.HashPasswordMD5(password);

                var employee = await _context.Employees
                    .Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.Login == login && e.Password == hashedPassword);

                if (employee != null)
                {
                    return (true, "Вход выполнен успешно!", employee);
                }

                return (false, "Неверный логин или пароль!", null);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ошибка авторизации: {ex.Message}", null);
            }
        }
    }
}
