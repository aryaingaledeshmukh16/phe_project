using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.DeScrutiny.Commands
{
    public interface ISaveDeScrutinyActionHandler
    {
        Task<Applicant> HandleAsync(string applicationNo, DeScrutinyActionCommand command, CancellationToken cancellationToken = default);
    }
}
