using PHE.API.Models;

namespace PHE.API.Application.Features.DeScrutiny.History
{
    public interface IGetDeScrutinyHistoryHandler
    {
        Task<List<DeScrutinyHistoryItem>> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
