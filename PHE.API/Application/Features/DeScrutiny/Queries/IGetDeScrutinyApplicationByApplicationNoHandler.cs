using PHE.API.Models;

namespace PHE.API.Application.Features.DeScrutiny.Queries
{
    public interface IGetDeScrutinyApplicationByApplicationNoHandler
    {
        Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
