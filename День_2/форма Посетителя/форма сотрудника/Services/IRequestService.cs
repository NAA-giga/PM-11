using System;
using System.Collections.Generic;
using System.Text;
using форма_сотрудника.Models;

namespace форма_сотрудника.Services
{
    public interface IRequestService
    {
        Task<List<Заявка>> GetRequestsByDepartment(int departmentId);
        Task<List<Заявка>> GetRequestsByEmployee(int employeeId);
        Task<Заявка> GetRequestDetails(int requestId);
        Task<(bool Success, string Message)> ApproveRequest(int requestId, int employeeId);
        Task<(bool Success, string Message)> RejectRequest(int requestId, int employeeId, string reason);
    }
}
