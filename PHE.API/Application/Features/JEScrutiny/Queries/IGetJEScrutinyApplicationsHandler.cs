using PHE.API.Models;

namespace PHE.API.Application.Features.JEScrutiny.Queries
{
    public interface IGetJEScrutinyApplicationsHandler
    {
        Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default);
    }
}
