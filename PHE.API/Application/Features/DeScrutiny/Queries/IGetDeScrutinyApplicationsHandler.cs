using PHE.API.Models;

namespace PHE.API.Application.Features.DeScrutiny.Queries
{
    public interface IGetDeScrutinyApplicationsHandler
    {
        Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default);
    }
}
