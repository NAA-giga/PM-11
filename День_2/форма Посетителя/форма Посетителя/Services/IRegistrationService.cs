using System;
using System.Collections.Generic;
using System.Text;
using форма_Посетителя.Models;

namespace форма_Посетителя.Services
{
    public interface IRegistrationService
    {
        Task<(bool Success, string Message, int VisitorId)> RegisterWithSQL(RegisterModel model);
        Task<(bool Success, string Message, int VisitorId)> RegisterWithStoredProcedure(RegisterModel model);
        Task<(bool Success, string Message, int VisitorId)> RegisterWithEF(RegisterModel model);
    }
}
