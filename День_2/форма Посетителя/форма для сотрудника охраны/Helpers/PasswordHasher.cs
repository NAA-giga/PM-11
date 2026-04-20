using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace форма_для_сотрудника_охраны.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPasswordMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }
    }
}
