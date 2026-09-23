using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public class SendApplicantOtpHandler : ISendApplicantOtpHandler
    {
        private readonly IApplicantWriteService _applicantWriteService;

        public SendApplicantOtpHandler(IApplicantWriteService applicantWriteService)
        {
            _applicantWriteService = applicantWriteService;
        }

        public async Task<Applicant> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            return await _applicantWriteService.SendOtpAsync(applicationNo, cancellationToken);
        }
    }
}
