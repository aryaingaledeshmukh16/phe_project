using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.DeScrutiny.Commands
{
    public class SaveDeScrutinyActionHandler : ISaveDeScrutinyActionHandler
    {
        private readonly IDeScrutinyWriteService _deScrutinyWriteService;

        public SaveDeScrutinyActionHandler(IDeScrutinyWriteService deScrutinyWriteService)
        {
            _deScrutinyWriteService = deScrutinyWriteService;
        }

        public async Task<Applicant> HandleAsync(string applicationNo, DeScrutinyActionCommand command, CancellationToken cancellationToken = default)
        {
            return await _deScrutinyWriteService.SaveActionAsync(applicationNo, command, cancellationToken);
        }
    }
}
