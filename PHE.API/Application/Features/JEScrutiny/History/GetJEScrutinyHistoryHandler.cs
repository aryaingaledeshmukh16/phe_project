using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.JEScrutiny.History
{
    public class GetJEScrutinyHistoryHandler : IGetJEScrutinyHistoryHandler
    {
        private readonly IJEScrutinyReadService _jeScrutinyReadService;

        public GetJEScrutinyHistoryHandler(IJEScrutinyReadService jeScrutinyReadService)
        {
            _jeScrutinyReadService = jeScrutinyReadService;
        }

        public async Task<List<JEScrutinyHistoryItem>> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return new List<JEScrutinyHistoryItem>();
            }

            return await _jeScrutinyReadService.GetHistoryByApplicationNoAsync(applicationNo, cancellationToken);
        }
    }
}
