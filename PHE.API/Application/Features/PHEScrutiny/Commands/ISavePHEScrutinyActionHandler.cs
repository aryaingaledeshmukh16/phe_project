using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.PHEScrutiny.Commands
{
    public interface ISavePHEScrutinyActionHandler
    {
        Task<Applicant> HandleAsync(string applicationNo, PHEScrutinyActionCommand command, CancellationToken cancellationToken = default);
    }
}
