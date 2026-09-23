using PHE.API.Models;

namespace PHE.API.Application.Common.Interfaces
{
    public interface IPHEScrutinyHistoryReadService
    {
        Task<List<PHEHistoryItem>> GetByApplicationNoAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
