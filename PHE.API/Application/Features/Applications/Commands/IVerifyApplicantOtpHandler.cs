using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public interface IVerifyApplicantOtpHandler
    {
        Task<Applicant> HandleAsync(string applicationNo, string otp, CancellationToken cancellationToken = default);
    }
}
