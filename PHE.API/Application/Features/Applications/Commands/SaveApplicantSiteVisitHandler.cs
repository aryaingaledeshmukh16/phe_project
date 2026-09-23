using PHE.API.Application.Common.Interfaces;
using PHE.API.DTOs;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public class SaveApplicantSiteVisitHandler : ISaveApplicantSiteVisitHandler
    {
        private readonly IApplicantWriteService _applicantWriteService;

        public SaveApplicantSiteVisitHandler(IApplicantWriteService applicantWriteService)
        {
            _applicantWriteService = applicantWriteService;
        }

        public async Task<Applicant> HandleAsync(string applicationNo, SiteVisitDto siteVisit, CancellationToken cancellationToken = default)
        {
            return await _applicantWriteService.SaveSiteVisitAsync(applicationNo, siteVisit, cancellationToken);
        }
    }
}
