using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Infrastructure.Services
{
    public class DeScrutinyHistoryReadService : IDeScrutinyHistoryReadService
    {
        private readonly ApplicationDbContext _context;

        public DeScrutinyHistoryReadService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeScrutinyHistoryItem>> GetByApplicationNoAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            return await _context.ApplicantsLogs
                .AsNoTracking()
                .Where(x => x.ApplicationNo == applicationNo)
                .OrderByDescending(x => x.LogId)
                .Select(x => new DeScrutinyHistoryItem
                {
                    logId = x.LogId,
                    applicationNo = x.ApplicationNo,
                    role = x.role,
                    userCode = x.user_code,
                    userName = x.user_name,
                    scrutinyStatus = x.scrutiny_status,
                    applicationStatus = x.Application_status,
                    remark = x.Remark,
                    entryDate = x.entry_date
                })
                .ToListAsync(cancellationToken);
        }
    }
}
