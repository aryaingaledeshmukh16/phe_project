using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.Applications.Commands
{
    public interface IUpdateApplicantLayoutHandler
    {
        Task<Applicant> HandleAsync(string applicationNo, LayoutUpdateCommand command, CancellationToken cancellationToken = default);
    }
}
