using PHE.API.Application.Common.Interfaces;
using PHE.API.DTOs;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public class UploadApplicantDocumentsHandler : IUploadApplicantDocumentsHandler
    {
        private readonly IApplicantWriteService _applicantWriteService;

        public UploadApplicantDocumentsHandler(IApplicantWriteService applicantWriteService)
        {
            _applicantWriteService = applicantWriteService;
        }

        public async Task<Applicant> HandleAsync(string applicationNo, UploadDocumentsDto documents, CancellationToken cancellationToken = default)
        {
            return await _applicantWriteService.UploadDocumentsAsync(applicationNo, documents, cancellationToken);
        }
    }
}
