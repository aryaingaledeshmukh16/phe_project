using PHE.API.Models;

namespace PHE.API.Application.Common.Interfaces
{
    public interface IJEScrutinyReadService
    {
        Task<Applicant?> GetByApplicationNoAsync(string applicationNo, CancellationToken cancellationToken = default);
        Task<List<JEScrutinyHistoryItem>> GetHistoryByApplicationNoAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
