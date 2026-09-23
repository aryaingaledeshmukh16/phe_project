using PHE.API.Models;

namespace PHE.API.Application.Features.JEApplications.Queries
{
    public interface IGetJEApplicationByApplicationNoHandler
    {
        Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
