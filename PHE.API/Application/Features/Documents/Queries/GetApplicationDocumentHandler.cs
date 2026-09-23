using Microsoft.AspNetCore.Mvc;
using PHE.API.Application.Common.Interfaces;

namespace PHE.API.Application.Features.Documents.Queries
{
    public class GetApplicationDocumentHandler : IGetApplicationDocumentHandler
    {
        private readonly IDocumentReadService _documentReadService;

        public GetApplicationDocumentHandler(IDocumentReadService documentReadService)
        {
            _documentReadService = documentReadService;
        }

        public async Task<IActionResult> HandleAsync(string applicationNo, string documentType, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return new BadRequestObjectResult(new
                {
                    message = "Application number is required."
                });
            }

            if (string.IsNullOrWhiteSpace(documentType))
            {
                return new BadRequestObjectResult(new
                {
                    message = "Document type is required."
                });
            }

            var result = await _documentReadService.GetApplicationDocumentAsync(applicationNo, documentType, cancellationToken);

            if (result.Result != null)
            {
                return result.Result;
            }

            if (result.Message == "Application not found." || result.Message == "Document not uploaded." || result.Message == "Document file not found on server.")
            {
                return new NotFoundObjectResult(new
                {
                    message = result.Message,
                    documentType = documentType
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
                message = result.Message ?? "Document not uploaded."
            });
        }
    }
}
