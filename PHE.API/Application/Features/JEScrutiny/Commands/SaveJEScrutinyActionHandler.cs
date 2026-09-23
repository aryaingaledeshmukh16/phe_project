using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.JEScrutiny.Commands
{
    public class SaveJEScrutinyActionHandler : ISaveJEScrutinyActionHandler
    {
        private readonly IJEScrutinyWriteService _jeScrutinyWriteService;

        public SaveJEScrutinyActionHandler(IJEScrutinyWriteService jeScrutinyWriteService)
        {
            _jeScrutinyWriteService = jeScrutinyWriteService;
        }

        public async Task<Applicant> HandleAsync(string applicationNo, JEScrutinyActionCommand command, CancellationToken cancellationToken = default)
        {
            return await _jeScrutinyWriteService.SaveActionAsync(applicationNo, command, cancellationToken);
        }
    }
}
