using PHE.API.Models;

namespace PHE.API.Application.Common.Interfaces
{
    public interface IDeScrutinyReadService
    {
        Task<List<Applicant>> GetApplicationsAsync(CancellationToken cancellationToken = default);
        Task<Applicant?> GetByApplicationNoAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
