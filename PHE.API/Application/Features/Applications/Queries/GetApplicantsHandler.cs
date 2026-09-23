using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Queries
{
    public class GetApplicantsHandler : IGetApplicantsHandler
    {
        private readonly IApplicantReadService _applicantReadService;

        public GetApplicantsHandler(IApplicantReadService applicantReadService)
        {
            _applicantReadService = applicantReadService;
        }

        public async Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _applicantReadService.GetApplicationsAsync(cancellationToken);
        }
    }
}
