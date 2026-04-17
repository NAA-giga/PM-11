using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace форма_Посетителя.Helpers
{
    public static class PasswordHasher
    {
        /// <summary>
        /// Хеширование пароля с использованием MD5 (согласно ТЗ)
        /// </summary>
        public static string HashPasswordMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }

        /// <summary>
        /// Хеширование пароля с использованием SHA256 (более безопасно)
        /// </summary>
        public static string HashPasswordSHA256(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }
    }
}
