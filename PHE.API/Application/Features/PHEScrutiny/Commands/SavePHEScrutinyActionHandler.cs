using PHE.API.Application.Common.Interfaces;
using PHE.API.Models;

namespace PHE.API.Application.Features.PHEScrutiny.Commands
{
    public class SavePHEScrutinyActionHandler : ISavePHEScrutinyActionHandler
    {
        private readonly IPHEScrutinyWriteService _pheScrutinyWriteService;

        public SavePHEScrutinyActionHandler(IPHEScrutinyWriteService pheScrutinyWriteService)
        {
            _pheScrutinyWriteService = pheScrutinyWriteService;
        }

        public async Task<Applicant> HandleAsync(string applicationNo, PHEScrutinyActionCommand command, CancellationToken cancellationToken = default)
        {
            return await _pheScrutinyWriteService.SaveActionAsync(applicationNo, command, cancellationToken);
        }
    }
}
