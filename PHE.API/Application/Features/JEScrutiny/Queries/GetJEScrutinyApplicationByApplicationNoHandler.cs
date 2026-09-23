using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.JEScrutiny.Queries
{
    public class GetJEScrutinyApplicationByApplicationNoHandler : IGetJEScrutinyApplicationByApplicationNoHandler
    {
        private readonly IJEScrutinyReadService _jeScrutinyReadService;

        public GetJEScrutinyApplicationByApplicationNoHandler(IJEScrutinyReadService jeScrutinyReadService)
        {
            _jeScrutinyReadService = jeScrutinyReadService;
        }

        public async Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return null;
            }

            return await _jeScrutinyReadService.GetByApplicationNoAsync(applicationNo, cancellationToken);
        }
    }
}
