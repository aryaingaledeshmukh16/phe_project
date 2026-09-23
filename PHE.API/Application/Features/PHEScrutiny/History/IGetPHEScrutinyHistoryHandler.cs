using PHE.API.Models;

namespace PHE.API.Application.Features.PHEScrutiny.History
{
    public interface IGetPHEScrutinyHistoryHandler
    {
        Task<List<PHEHistoryItem>> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
