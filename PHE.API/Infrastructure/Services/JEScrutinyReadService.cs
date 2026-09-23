using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Infrastructure.Services
{
    public class JEScrutinyReadService : IJEScrutinyReadService
    {
        private readonly ApplicationDbContext _context;

        public JEScrutinyReadService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Applicant?> GetByApplicationNoAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return null;
            }

            return await _context.Applicants
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);
        }

        public async Task<List<JEScrutinyHistoryItem>> GetHistoryByApplicationNoAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            return await _context.ApplicantsLogs
                .AsNoTracking()
                .Where(x => x.ApplicationNo == applicationNo)
                .OrderByDescending(x => x.LogId)
                .Select(x => new JEScrutinyHistoryItem
                {
                    logId = x.LogId,
                    applicationNo = x.ApplicationNo ?? string.Empty,
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
