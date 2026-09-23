using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public class VerifyApplicantOtpHandler : IVerifyApplicantOtpHandler
    {
        private readonly IApplicantWriteService _applicantWriteService;

        public VerifyApplicantOtpHandler(IApplicantWriteService applicantWriteService)
        {
            _applicantWriteService = applicantWriteService;
        }

        public async Task<Applicant> HandleAsync(string applicationNo, string otp, CancellationToken cancellationToken = default)
        {
            return await _applicantWriteService.VerifyOtpAsync(applicationNo, otp, cancellationToken);
        }
    }
}
