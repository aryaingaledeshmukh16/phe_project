using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.JEApplications.Queries
{
    public class GetJEApplicationsHandler : IGetJEApplicationsHandler
    {
        private readonly IJEApplicationReadService _jeApplicationReadService;

        public GetJEApplicationsHandler(IJEApplicationReadService jeApplicationReadService)
        {
            _jeApplicationReadService = jeApplicationReadService;
        }

        public async Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _jeApplicationReadService.GetApplicationsAsync(cancellationToken);
        }
    }
}
