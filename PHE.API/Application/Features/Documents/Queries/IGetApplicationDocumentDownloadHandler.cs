using Microsoft.AspNetCore.Mvc;

namespace PHE.API.Application.Features.Documents.Queries
{
    public interface IGetApplicationDocumentDownloadHandler
    {
        Task<IActionResult> HandleAsync(string applicationNo, string documentType, CancellationToken cancellationToken = default);
    }
}
