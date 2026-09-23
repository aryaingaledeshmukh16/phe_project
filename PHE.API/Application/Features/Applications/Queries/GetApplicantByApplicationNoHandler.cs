using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Queries
{
    public class GetApplicantByApplicationNoHandler : IGetApplicantByApplicationNoHandler
    {
        private readonly IApplicantReadService _applicantReadService;

        public GetApplicantByApplicationNoHandler(IApplicantReadService applicantReadService)
        {
            _applicantReadService = applicantReadService;
        }

        public async Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return null;
            }

            return await _applicantReadService.GetByApplicationNoAsync(applicationNo, cancellationToken);
        }
    }
}
