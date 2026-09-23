using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.DeScrutiny.Queries
{
    public class GetDeScrutinyApplicationsHandler : IGetDeScrutinyApplicationsHandler
    {
        private readonly IDeScrutinyReadService _deScrutinyReadService;

        public GetDeScrutinyApplicationsHandler(IDeScrutinyReadService deScrutinyReadService)
        {
            _deScrutinyReadService = deScrutinyReadService;
        }

        public async Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _deScrutinyReadService.GetApplicationsAsync(cancellationToken);
        }
    }
}
