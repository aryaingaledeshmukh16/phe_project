using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.PHEScrutiny.History
{
    public class GetPHEScrutinyHistoryHandler : IGetPHEScrutinyHistoryHandler
    {
        private readonly IPHEScrutinyHistoryReadService _historyReadService;

        public GetPHEScrutinyHistoryHandler(IPHEScrutinyHistoryReadService historyReadService)
        {
            _historyReadService = historyReadService;
        }

        public async Task<List<PHEHistoryItem>> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return new List<PHEHistoryItem>();
            }

            return await _historyReadService.GetByApplicationNoAsync(applicationNo, cancellationToken);
        }
    }
}
