using PHE.API.Application.Common.Interfaces;

namespace PHE.API.Application.Features.JEScrutiny.Queries
{
    public class GetJEScrutinyNocHandler : IGetJEScrutinyNocHandler
    {
        private readonly INocService _nocService;

        public GetJEScrutinyNocHandler(INocService nocService)
        {
            _nocService = nocService;
        }

        public async Task<JEScrutinyNocResult> HandleAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            return await _nocService.GetWaterNocAsync(applicationNo, cancellationToken);
        }
    }
}
