using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public class UpdateApplicantLayoutHandler : IUpdateApplicantLayoutHandler
    {
        private readonly IApplicantWriteService _applicantWriteService;

        public UpdateApplicantLayoutHandler(IApplicantWriteService applicantWriteService)
        {
            _applicantWriteService = applicantWriteService;
        }

        public async Task<Applicant> HandleAsync(string applicationNo, LayoutUpdateCommand command, CancellationToken cancellationToken = default)
        {
            return await _applicantWriteService.UpdateLayoutAsync(applicationNo, command, cancellationToken);
        }
    }
}
