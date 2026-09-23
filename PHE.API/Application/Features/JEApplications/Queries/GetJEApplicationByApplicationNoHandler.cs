using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.JEApplications.Queries
{
    public class GetJEApplicationByApplicationNoHandler : IGetJEApplicationByApplicationNoHandler
    {
        private readonly IJEApplicationReadService _jeApplicationReadService;

        public GetJEApplicationByApplicationNoHandler(IJEApplicationReadService jeApplicationReadService)
        {
            _jeApplicationReadService = jeApplicationReadService;
        }

        public async Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return null;
            }

            return await _jeApplicationReadService.GetByApplicationNoAsync(applicationNo, cancellationToken);
        }
    }
}
