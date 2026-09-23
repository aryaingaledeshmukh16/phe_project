using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Queries
{
    public interface IGetApplicantByApplicationNoHandler
    {
        Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
