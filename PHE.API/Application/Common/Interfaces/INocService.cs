using PHE.API.Application.Features.JEScrutiny.Queries;

namespace PHE.API.Application.Common.Interfaces
{
    public interface INocService
    {
        Task<JEScrutinyNocResult> GetWaterNocAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
