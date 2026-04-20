using System;
using System.Collections.Generic;
using System.Text;
using форма_для_сотрудника_охраны.Models;

namespace форма_для_сотрудника_охраны.Services
{
    public interface IRequestService
    {
        Task<List<Заявка>> GetApprovedRequests();
        Task<List<Заявка>> GetApprovedRequestsByDate(DateOnly date);
        Task<List<Заявка>> GetApprovedRequestsByDepartment(int departmentId);
        Task<Заявка> GetRequestDetails(int requestId);
        Task<List<Заявка>> SearchRequests(string searchTerm);
        Task<(bool Success, string Message)> GrantAccess(int requestId, int employeeId);
        Task<(bool Success, string Message)> RecordExit(int requestId, int employeeId);
        Task<List<DepartmentItem>> GetDepartments();
    }
}
