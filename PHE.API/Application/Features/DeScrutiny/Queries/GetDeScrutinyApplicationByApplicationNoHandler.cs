using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.DeScrutiny.Queries
{
    public class GetDeScrutinyApplicationByApplicationNoHandler : IGetDeScrutinyApplicationByApplicationNoHandler
    {
        private readonly IDeScrutinyReadService _deScrutinyReadService;

        public GetDeScrutinyApplicationByApplicationNoHandler(IDeScrutinyReadService deScrutinyReadService)
        {
            _deScrutinyReadService = deScrutinyReadService;
        }

        public async Task<Applicant?> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return null;
            }

            return await _deScrutinyReadService.GetByApplicationNoAsync(applicationNo, cancellationToken);
        }
    }
}
