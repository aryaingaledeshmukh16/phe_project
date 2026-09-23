using PHE.API.DTOs;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public interface IUploadApplicantDocumentsHandler
    {
        Task<Applicant> HandleAsync(string applicationNo, UploadDocumentsDto documents, CancellationToken cancellationToken = default);
    }
}
