using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using форма_Посетителя.Models;

namespace форма_Посетителя.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public RequestService(ApplicationDbContext context)
        {
            _context = context;
            _connectionString = _context.Database.GetConnectionString();
        }

        public async Task<List<DepartmentItem>> GetDepartments()
        {
            var departments = await _context.Подразделениеs
                .Select(d => new DepartmentItem
                {
                    Id = d.Id,
                    Name = d.Наименование ?? string.Empty
                })
                .ToListAsync();

            return departments;
        }
        public async Task<(bool Success, string Message, int RequestId)> CreateGroupRequest(GroupRequestModel model, int visitorId, List<int> groupVisitorIds)
        {
            try
            {
                var request = new Заявка
                {
                    Наименование = $"Групповая заявка от {DateTime.Now:dd.MM.yyyy}",
                    ТипЗаявки = "групповая",
                    ДатаСозданиеЗаявки = DateTime.Now,
                    Цель = model.Purpose,
                    Статус = "проверка",
                    ДатаНачалоРеализацииЗаявки = DateOnly.FromDateTime(model.StartDate),
                    ДатаОкончаниеРеализацииЗаявки = DateOnly.FromDateTime(model.EndDate),
                    ПосетительId = visitorId,
                    СотрудникId = model.EmployeeId,
                    ПодразделениеId = model.DepartmentId
                };

                _context.Заявкаs.Add(request);
                await _context.SaveChangesAsync();

                return (true, "Групповая заявка успешно создана!", request.Id);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка создания групповой заявки: {ex.Message}", 0);
            }
        }

        public async Task<List<EmployeeItem>> GetEmployeesByDepartment(int departmentId)
        {
            var employees = await _context.Сотрудникs
                .Where(e => e.ПодразделениеId == departmentId)
                .Select(e => new EmployeeItem
                {
                    Id = e.Id,
                    FullName = $"{e.Фамилия} {e.Имя} {(string.IsNullOrEmpty(e.Отчество) ? "" : e.Отчество)}".Trim(),
                    Position = e.Примечание ?? ""
                })
                .ToListAsync();

            return employees;
        }

        public async Task<(bool Success, string Message, int RequestId)> CreateIndividualRequest(RequestModel model, int visitorId)
        {
            try
            {
                var request = new Заявка
                {
                    Наименование = $"Заявка от {DateTime.Now:dd.MM.yyyy}",
                    ТипЗаявки = "личная",
                    ДатаСозданиеЗаявки = DateTime.Now,
                    Цель = model.Purpose,
                    Статус = "проверка",
                    ДатаНачалоРеализацииЗаявки = DateOnly.FromDateTime(model.StartDate),
                    ДатаОкончаниеРеализацииЗаявки = DateOnly.FromDateTime(model.EndDate),
                    ПосетительId = visitorId,
                    СотрудникId = model.EmployeeId,
                    ПодразделениеId = model.DepartmentId
                };

                _context.Заявкаs.Add(request);
                await _context.SaveChangesAsync();

                return (true, "Заявка успешно создана!", request.Id);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка создания заявки: {ex.Message}", 0);
            }
        }

        public async Task<List<Заявка>> GetVisitorRequests(int visitorId)
        {
            var requests = await _context.Заявкаs
                .Include(r => r.Подразделение)
                .Include(r => r.Сотрудник)
                .Where(r => r.ПосетительId == visitorId)
                .OrderByDescending(r => r.ДатаСозданиеЗаявки)
                .ToListAsync();

            return requests;
        }

        public async Task<Заявка> GetRequestDetails(int requestId)
        {
            var request = await _context.Заявкаs
                .FirstOrDefaultAsync(r => r.Id == requestId);

            return request;
        }

        public async Task<Заявка> GetRequestWithDetails(int requestId)
        {
            var request = await _context.Заявкаs
                .Include(r => r.Подразделение)
                .Include(r => r.Сотрудник)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            return request;
        }
    }
}
