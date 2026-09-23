using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.JEScrutiny.Queries
{
    public class GetJEScrutinyApplicationsHandler : IGetJEScrutinyApplicationsHandler
    {
        private readonly IJEApplicationReadService _jeApplicationReadService;

        public GetJEScrutinyApplicationsHandler(IJEApplicationReadService jeApplicationReadService)
        {
            _jeApplicationReadService = jeApplicationReadService;
        }

        public async Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _jeApplicationReadService.GetApplicationsAsync(cancellationToken);
        }
    }
}
