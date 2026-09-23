using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Infrastructure.Services
{
    public class ApplicantReadService : IApplicantReadService
    {
        private readonly ApplicationDbContext _context;

        public ApplicantReadService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Applicant>> GetApplicationsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Applicants
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
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
    }
}
