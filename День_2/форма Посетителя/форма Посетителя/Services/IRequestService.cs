using System;
using System.Collections.Generic;
using System.Text;
using форма_Посетителя.Models;

namespace форма_Посетителя.Services
{
    public interface IRequestService
    {
        Task<List<DepartmentItem>> GetDepartments();
        Task<List<EmployeeItem>> GetEmployeesByDepartment(int departmentId);
        Task<(bool Success, string Message, int RequestId)> CreateIndividualRequest(RequestModel model, int visitorId);
        Task<List<Заявка>> GetVisitorRequests(int visitorId);
        Task<Заявка> GetRequestDetails(int requestId);
        Task<Заявка> GetRequestWithDetails(int requestId);
        Task<(bool Success, string Message, int RequestId)> CreateGroupRequest(GroupRequestModel model, int visitorId, List<int> groupVisitorIds);
    }
}
