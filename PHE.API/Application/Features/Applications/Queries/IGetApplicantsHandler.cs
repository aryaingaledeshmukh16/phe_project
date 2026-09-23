using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Queries
{
    public interface IGetApplicantsHandler
    {
        Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default);
    }
}
