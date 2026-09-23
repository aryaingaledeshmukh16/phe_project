using Microsoft.AspNetCore.Mvc;

namespace PHE.API.Application.Features.Documents.Queries
{
    public interface IGetJEScrutinyDocumentHandler
    {
        Task<IActionResult> HandleAsync(string path, bool download = false, CancellationToken cancellationToken = default);
    }
}
