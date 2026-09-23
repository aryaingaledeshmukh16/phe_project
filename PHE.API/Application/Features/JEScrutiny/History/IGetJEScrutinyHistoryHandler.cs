using PHE.API.Models;

namespace PHE.API.Application.Features.JEScrutiny.History
{
    public interface IGetJEScrutinyHistoryHandler
    {
        Task<List<JEScrutinyHistoryItem>> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
