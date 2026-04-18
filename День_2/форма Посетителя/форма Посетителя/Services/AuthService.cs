using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using форма_Посетителя.Helpers;
using форма_Посетителя.Models;

namespace форма_Посетителя.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Авторизация через Entity Framework Core по email и паролю
        /// </summary>
        public async Task<(bool Success, string Message, Посетитель? Visitor)> LoginWithEF(string email, string password)
        {
            try
            {
                string hashedPassword = PasswordHasher.HashPasswordMD5(password);

                var visitor = await _context.Посетительs
                    .FirstOrDefaultAsync(v => v.Email == email && v.Пароль == hashedPassword);

                if (visitor != null)
                {
                    return (true, "Вход выполнен успешно!", visitor);
                }

                return (false, "Неверный email или пароль!", null);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ошибка авторизации: {ex.Message}", null);
            }
        }
    }
}
