using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using форма_для_сотрудника_охраны.Data;
using форма_для_сотрудника_охраны.Helpers;
using форма_для_сотрудника_охраны.Models;

namespace форма_для_сотрудника_охраны.Services
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

        public async Task<List<Заявка>> GetApprovedRequests()
        {
            using (var context = CreateContext())
            {
                var requests = await context.Requests
                    .Include(r => r.Visitor)
                    .Include(r => r.Department)
                    .Include(r => r.Employee)
                    .Where(r => r.Status == "одобрена")
                    .OrderByDescending(r => r.StartDate)
                    .ToListAsync();

                return requests;
            }
        }

        public async Task<List<Заявка>> GetApprovedRequestsByDate(DateOnly date)
        {
            using (var context = CreateContext())
            {
                var requests = await context.Requests
                    .Include(r => r.Visitor)
                    .Include(r => r.Department)
                    .Include(r => r.Employee)
                    .Where(r => r.Status == "одобрена" && r.StartDate == date)
                    .OrderByDescending(r => r.StartDate)
                    .ToListAsync();

                return requests;
            }
        }

        public async Task<List<Заявка>> GetApprovedRequestsByDepartment(int departmentId)
        {
            using (var context = CreateContext())
            {
                var requests = await context.Requests
                    .Include(r => r.Visitor)
                    .Include(r => r.Department)
                    .Include(r => r.Employee)
                    .Where(r => r.Status == "одобрена" && r.DepartmentId == departmentId)
                    .OrderByDescending(r => r.StartDate)
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

        public async Task<List<Заявка>> SearchRequests(string searchTerm)
        {
            using (var context = CreateContext())
            {
                var requests = await context.Requests
                    .Include(r => r.Visitor)
                    .Include(r => r.Department)
                    .Include(r => r.Employee)
                    .Where(r => r.Status == "одобрена" &&
                        (r.Visitor.LastName.Contains(searchTerm) ||
                         r.Visitor.FirstName.Contains(searchTerm) ||
                         r.Visitor.PassportNumber.Contains(searchTerm)))
                    .ToListAsync();

                return requests;
            }
        }

        public async Task<(bool Success, string Message)> GrantAccess(int requestId, int employeeId)
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

                    if (request.EntryTime != null)
                    {
                        return (false, "Доступ уже был предоставлен");
                    }

                    request.EntryTime = DateTime.Now;
                    request.EmployeeId = employeeId;

                    await context.SaveChangesAsync();

                    SoundHelper.PlaySystemSound();

                    return (true, $"Доступ предоставлен в {DateTime.Now:HH:mm:ss}");
                }
                catch (Exception ex)
                {
                    return (false, $"Ошибка: {ex.Message}");
                }
            }
        }

        public async Task<(bool Success, string Message)> RecordExit(int requestId, int employeeId)
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

                    if (request.EntryTime == null)
                    {
                        return (false, "Сначала необходимо зафиксировать вход");
                    }

                    if (request.ExitTime != null)
                    {
                        return (false, "Убытие уже зафиксировано");
                    }

                    request.ExitTime = DateTime.Now;
                    request.EmployeeId = employeeId;

                    await context.SaveChangesAsync();

                    return (true, $"Убытие зафиксировано в {DateTime.Now:HH:mm:ss}");
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
    }
}
