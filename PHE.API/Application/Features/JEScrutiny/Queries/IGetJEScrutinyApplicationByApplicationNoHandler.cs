using PHE.API.Models;

namespace PHE.API.Application.Features.JEScrutiny.Queries
{
    public interface IGetJEScrutinyApplicationByApplicationNoHandler
    {
        Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
