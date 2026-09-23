using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.PHEScrutiny.Queries
{
    public class GetPHEScrutinyApplicationByApplicationNoHandler : IGetPHEScrutinyApplicationByApplicationNoHandler
    {
        private readonly IPHEScrutinyReadService _pheScrutinyReadService;

        public GetPHEScrutinyApplicationByApplicationNoHandler(IPHEScrutinyReadService pheScrutinyReadService)
        {
            _pheScrutinyReadService = pheScrutinyReadService;
        }

        public async Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return null;
            }

            return await _pheScrutinyReadService.GetByApplicationNoAsync(applicationNo, cancellationToken);
        }
    }
}
