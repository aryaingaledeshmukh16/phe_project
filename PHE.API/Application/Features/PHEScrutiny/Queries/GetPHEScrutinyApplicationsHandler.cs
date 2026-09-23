using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.PHEScrutiny.Queries
{
    public class GetPHEScrutinyApplicationsHandler : IGetPHEScrutinyApplicationsHandler
    {
        private readonly IPHEScrutinyReadService _pheScrutinyReadService;

        public GetPHEScrutinyApplicationsHandler(IPHEScrutinyReadService pheScrutinyReadService)
        {
            _pheScrutinyReadService = pheScrutinyReadService;
        }

        public async Task<List<Applicant>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _pheScrutinyReadService.GetApplicationsAsync(cancellationToken);
        }
    }
}
