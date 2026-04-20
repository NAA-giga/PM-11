using System;
using System.Collections.Generic;
using System.Text;
using форма_для_сотрудника_охраны.Models;

namespace форма_для_сотрудника_охраны.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, Employee? Employee)> LoginByCode(string login, string password);
    }
}
