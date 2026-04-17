using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using форма_сотрудника.Data;
using форма_сотрудника.Models;
using Microsoft.EntityFrameworkCore;

namespace форма_сотрудника.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _context;

        public RequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Заявка>> GetRequestsByDepartment(int departmentId)
        {
            var requests = await _context.Requests
                .Include(r => r.Visitor)
                .Include(r => r.Department)
                .Include(r => r.Employee)
                .Where(r => r.DepartmentId == departmentId && r.Status == "проверка")
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();

            return requests;
        }

        public async Task<List<Заявка>> GetRequestsByEmployee(int employeeId)
        {
            var requests = await _context.Requests
                .Include(r => r.Visitor)
                .Include(r => r.Department)
                .Include(r => r.Employee)
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();

            return requests;
        }

        public async Task<Заявка> GetRequestDetails(int requestId)
        {
            var request = await _context.Requests
                .Include(r => r.Visitor)
                .Include(r => r.Department)
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            return request;
        }

        public async Task<(bool Success, string Message)> ApproveRequest(int requestId, int employeeId)
        {
            try
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request == null)
                {
                    return (false, "Заявка не найдена");
                }

                request.Status = "одобрена";
                request.ApprovalDate = DateTime.Now;
                request.EmployeeId = employeeId;

                await _context.SaveChangesAsync();
                return (true, "Заявка одобрена");
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RejectRequest(int requestId, int employeeId, string reason)
        {
            try
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request == null)
                {
                    return (false, "Заявка не найдена");
                }

                request.Status = "не одобрена";
                request.ApprovalDate = DateTime.Now;
                request.EmployeeId = employeeId;
                request.RejectionReason = reason;

                await _context.SaveChangesAsync();
                return (true, "Заявка отклонена");
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка: {ex.Message}");
            }
        }
    }
}
