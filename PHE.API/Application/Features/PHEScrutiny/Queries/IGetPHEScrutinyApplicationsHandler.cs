using PHE.API.Models;

namespace PHE.API.Application.Features.PHEScrutiny.Queries
{
    public interface IGetPHEScrutinyApplicationsHandler
    {
        Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default);
    }
}
