using PHE.API.DTOs;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public interface ISaveApplicantSiteVisitHandler
    {
        Task<Applicant> HandleAsync(string applicationNo, SiteVisitDto siteVisit, CancellationToken cancellationToken = default);
    }
}
