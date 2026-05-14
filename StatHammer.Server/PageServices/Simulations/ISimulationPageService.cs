using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatHammer.Server.PageServices.Simulations
{
    public interface ISimulationPageService
    {
        Task<List<SelectListItem>> GetUnitSelectListAsync(CancellationToken cancellationToken = default);

        Task<SimulationRunViewModel> RunSimulationAsync(
            int unitAId,
            int unitBId,
            int simulationCount,
            int maxTurns,
            bool useParallel,
            int maxDegreeOfParallelism,
            bool saveResult,
            CancellationToken cancellationToken = default);
    }
}