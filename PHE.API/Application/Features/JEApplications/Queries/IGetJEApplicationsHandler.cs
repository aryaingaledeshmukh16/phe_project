using PHE.API.Models;

namespace PHE.API.Application.Features.JEApplications.Queries
{
    public interface IGetJEApplicationsHandler
    {
        Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default);
    }
}
