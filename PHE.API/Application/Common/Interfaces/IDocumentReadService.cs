using PHE.API.Application.Common.Models;

namespace PHE.API.Application.Common.Interfaces
{
    public interface IDocumentReadService
    {
        Task<DocumentReadResult> GetDocumentAsync(string path, bool download = false, CancellationToken cancellationToken = default);
        Task<DocumentReadResult> GetApplicationDocumentAsync(string applicationNo, string documentType, CancellationToken cancellationToken = default);
        Task<DocumentReadResult> GetApplicationDocumentDownloadAsync(string applicationNo, string documentType, CancellationToken cancellationToken = default);
    }
}
