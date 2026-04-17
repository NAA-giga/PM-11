using System;
using System.Collections.Generic;
using System.Text;
using форма_сотрудника.Models;

namespace форма_сотрудника.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, Employee? Employee)> Login(string login, string password);
    }
}
