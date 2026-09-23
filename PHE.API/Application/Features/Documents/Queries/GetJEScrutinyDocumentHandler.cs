using Microsoft.AspNetCore.Mvc;
using PHE.API.Application.Common.Interfaces;

namespace PHE.API.Application.Features.Documents.Queries
{
    public class GetJEScrutinyDocumentHandler : IGetJEScrutinyDocumentHandler
    {
        private readonly IDocumentReadService _documentReadService;

        public GetJEScrutinyDocumentHandler(IDocumentReadService documentReadService)
        {
            _documentReadService = documentReadService;
        }

        public async Task<IActionResult> HandleAsync(string path, bool download = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return new BadRequestObjectResult(new
                {
                    message = "Document path is required."
                });
            }

            var result = await _documentReadService.GetDocumentAsync(path, download, cancellationToken);

            if (result.Result != null)
            {
                return result.Result;
            }

            if (result.Message == "Invalid document path." || result.Message == "Invalid document location." || result.Message == "Document path is required.")
            {
                return new BadRequestObjectResult(new
                {
                    message = result.Message
                });
            }

            if (!string.IsNullOrWhiteSpace(result.Message) && result.Error != null)
            {
                return new ObjectResult(new
                {
                    message = result.Message,
                    error = result.Error
                }) { StatusCode = 500 };
            }

            return new NotFoundObjectResult(new
            {
                message = result.Message ?? "Document file not found on server."
            });
        }
    }
}
