using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.DeScrutiny.History
{
    public class GetDeScrutinyHistoryHandler : IGetDeScrutinyHistoryHandler
    {
        private readonly IDeScrutinyHistoryReadService _historyReadService;

        public GetDeScrutinyHistoryHandler(IDeScrutinyHistoryReadService historyReadService)
        {
            _historyReadService = historyReadService;
        }

        public async Task<List<DeScrutinyHistoryItem>> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return new List<DeScrutinyHistoryItem>();
            }

            return await _historyReadService.GetByApplicationNoAsync(applicationNo, cancellationToken);
        }
    }
}
