using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using форма_сотрудника.Data;
using форма_сотрудника.Models;

namespace форма_сотрудника.Services
{
    public class BlackListService : IBlackListService
    {
        private readonly string _connectionString;

        public BlackListService(ApplicationDbContext context)
        {
            _connectionString = context.Database.GetConnectionString();
        }

        private ApplicationDbContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(_connectionString);
            return new ApplicationDbContext(optionsBuilder.Options);
        }

        public async Task<bool> IsInBlackList(int visitorId)
        {
            using (var context = CreateContext())
            {
                return await context.BlackLists
                    .AnyAsync(b => b.VisitorId == visitorId);
            }
        }

        public async Task AddToBlackList(int visitorId, string reason)
        {
            using (var context = CreateContext())
            {
                var blackListEntry = new BlackList
                {
                    VisitorId = visitorId,
                    Reason = reason,
                    AddedDate = System.DateTime.Now
                };
                context.BlackLists.Add(blackListEntry);
                await context.SaveChangesAsync();
            }
        }

        public async Task CheckAndAddToBlackList(int visitorId)
        {
            using (var context = CreateContext())
            {
                var rejectCount = await context.Requests
                    .CountAsync(r => r.VisitorId == visitorId
                                     && r.Status == "не одобрена"
                                     && r.RejectionReason != null
                                     && r.RejectionReason.Contains("недостоверные данные"));

                if (rejectCount >= 2)
                {
                    bool alreadyInBlackList = await context.BlackLists
                        .AnyAsync(b => b.VisitorId == visitorId);

                    if (!alreadyInBlackList)
                    {
                        var blackListEntry = new BlackList
                        {
                            VisitorId = visitorId,
                            Reason = "Двукратное указание недостоверных данных при оформлении заявки",
                            AddedDate = System.DateTime.Now
                        };
                        context.BlackLists.Add(blackListEntry);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
