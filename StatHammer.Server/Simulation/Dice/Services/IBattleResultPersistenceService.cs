using StatHammer.Server.Models.Entities;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;

namespace StatHammer.Server.Simulation.Dice.Services
{
    public interface IBattleResultPersistenceService
    {
        Task<SimulationResult> SaveBatchResultAsync(
            int unitAId,
            int unitBId,
            BattleSimulationBatchResult batchResult,
            CancellationToken cancellationToken = default);
    }

}
