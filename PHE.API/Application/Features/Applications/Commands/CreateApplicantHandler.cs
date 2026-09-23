using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public class CreateApplicantHandler : ICreateApplicantHandler
    {
        private readonly IApplicantWriteService _applicantWriteService;

        public CreateApplicantHandler(IApplicantWriteService applicantWriteService)
        {
            _applicantWriteService = applicantWriteService;
        }

        public async Task<Applicant> HandleAsync(Applicant applicant, CancellationToken cancellationToken = default)
        {
            return await _applicantWriteService.CreateAsync(applicant, cancellationToken);
        }
    }
}
