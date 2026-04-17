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
        /// Авторизация через Entity Framework Core по логину и паролю
        /// </summary>
        public async Task<(bool Success, string Message, Посетитель? Visitor)> LoginWithEF(string login, string password)
        {
            try
            {
                // Хешируем введённый пароль для сравнения с хешем в БД
                string hashedPassword = PasswordHasher.HashPasswordMD5(password);

                // Ищем посетителя по логину и хешу пароля
                var visitor = await _context.Посетительs
                    .FirstOrDefaultAsync(v => v.Логин == login && v.Пароль == hashedPassword);

                if (visitor != null)
                {
                    return (true, "Вход выполнен успешно!", visitor);
                }

                // Если не нашли по логину, пробуем найти по email
                var visitorByEmail = await _context.Посетительs
                    .FirstOrDefaultAsync(v => v.Email == login && v.Пароль == hashedPassword);

                if (visitorByEmail != null)
                {
                    return (true, "Вход выполнен успешно!", visitorByEmail);
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
