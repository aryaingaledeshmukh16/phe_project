using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.JEScrutiny.Commands
{
    public interface ISaveJEScrutinyActionHandler
    {
        Task<Applicant> HandleAsync(string applicationNo, JEScrutinyActionCommand command, CancellationToken cancellationToken = default);
    }
}
