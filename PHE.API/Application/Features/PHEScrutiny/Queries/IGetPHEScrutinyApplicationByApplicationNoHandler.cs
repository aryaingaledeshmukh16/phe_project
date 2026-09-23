using PHE.API.Models;

namespace PHE.API.Application.Features.PHEScrutiny.Queries
{
    public interface IGetPHEScrutinyApplicationByApplicationNoHandler
    {
        Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
