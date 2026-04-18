using System;
using System.Collections.Generic;
using System.Text;

namespace форма_сотрудника.Services
{
    public interface IBlackListService
    {
        Task<bool> IsInBlackList(int visitorId);
        Task AddToBlackList(int visitorId, string reason);
        Task CheckAndAddToBlackList(int visitorId);
    }
}
