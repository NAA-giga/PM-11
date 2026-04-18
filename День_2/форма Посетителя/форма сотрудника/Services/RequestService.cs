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
        private readonly string _connectionString;

        public RequestService(ApplicationDbContext context)
        {
            _connectionString = context.Database.GetConnectionString();
        }

        private ApplicationDbContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(_connectionString);
            return new ApplicationDbContext(optionsBuilder.Options);
        }

        public async Task<List<Заявка>> GetAllPendingRequests()
        {
            using (var context = CreateContext())
            {
                var requests = await context.Requests
                    .Include(r => r.Visitor)
                    .Include(r => r.Department)
                    .Include(r => r.Employee)
                    .Where(r => r.Status == "проверка")
                    .OrderByDescending(r => r.CreationDate)
                    .ToListAsync();

                return requests;
            }
        }

        public async Task<Заявка> GetRequestDetails(int requestId)
        {
            using (var context = CreateContext())
            {
                var request = await context.Requests
                    .Include(r => r.Visitor)
                    .Include(r => r.Department)
                    .Include(r => r.Employee)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                return request;
            }
        }

        public async Task<(bool Success, string Message)> ApproveRequest(int requestId, int employeeId)
        {
            using (var context = CreateContext())
            {
                try
                {
                    var request = await context.Requests.FindAsync(requestId);
                    if (request == null)
                    {
                        return (false, "Заявка не найдена");
                    }

                    request.Status = "одобрена";
                    request.ApprovalDate = DateTime.Now;
                    request.EmployeeId = employeeId;

                    await context.SaveChangesAsync();
                    return (true, "Заявка одобрена");
                }
                catch (Exception ex)
                {
                    return (false, $"Ошибка: {ex.Message}");
                }
            }
        }

        public async Task<(bool Success, string Message)> RejectRequest(int requestId, int employeeId, string reason)
        {
            using (var context = CreateContext())
            {
                try
                {
                    var request = await context.Requests.FindAsync(requestId);
                    if (request == null)
                    {
                        return (false, "Заявка не найдена");
                    }

                    request.Status = "не одобрена";
                    request.ApprovalDate = DateTime.Now;
                    request.EmployeeId = employeeId;
                    request.RejectionReason = reason;

                    await context.SaveChangesAsync();
                    return (true, "Заявка отклонена");
                }
                catch (Exception ex)
                {
                    return (false, $"Ошибка: {ex.Message}");
                }
            }
        }

        public async Task<List<DepartmentItem>> GetDepartments()
        {
            using (var context = CreateContext())
            {
                var departments = await context.Departments
                    .Select(d => new DepartmentItem
                    {
                        Id = d.Id,
                        Name = d.Name
                    })
                    .ToListAsync();

                return departments;
            }
        }

        // Остальные методы, если нужны
        public async Task<List<Заявка>> GetRequestsByDepartment(int departmentId)
        {
            using (var context = CreateContext())
            {
                var requests = await context.Requests
                    .Include(r => r.Visitor)
                    .Include(r => r.Department)
                    .Include(r => r.Employee)
                    .Where(r => r.DepartmentId == departmentId && r.Status == "проверка")
                    .OrderByDescending(r => r.CreationDate)
                    .ToListAsync();

                return requests;
            }
        }

        public async Task<List<Заявка>> GetRequestsByEmployee(int employeeId)
        {
            using (var context = CreateContext())
            {
                var requests = await context.Requests
                    .Include(r => r.Visitor)
                    .Include(r => r.Department)
                    .Include(r => r.Employee)
                    .Where(r => r.EmployeeId == employeeId)
                    .OrderByDescending(r => r.CreationDate)
                    .ToListAsync();

                return requests;
            }
        }
    }
}
