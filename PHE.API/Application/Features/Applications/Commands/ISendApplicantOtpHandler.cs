using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public interface ISendApplicantOtpHandler
    {
        Task<Applicant> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
