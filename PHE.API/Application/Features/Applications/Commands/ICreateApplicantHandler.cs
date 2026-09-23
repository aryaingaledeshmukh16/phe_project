using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public interface ICreateApplicantHandler
    {
        Task<Applicant> HandleAsync(Applicant applicant, CancellationToken cancellationToken = default);
    }
}
