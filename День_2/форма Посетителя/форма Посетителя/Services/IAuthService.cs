using System;
using System.Collections.Generic;
using System.Text;
using форма_Посетителя.Models;

namespace форма_Посетителя.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, Посетитель? Visitor)> LoginWithEF(string login, string password);
    }
}
