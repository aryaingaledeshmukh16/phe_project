using PHE.API.Models;

namespace PHE.API.Application.Common.Interfaces
{
    public interface IDeScrutinyHistoryReadService
    {
        Task<List<DeScrutinyHistoryItem>> GetByApplicationNoAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
