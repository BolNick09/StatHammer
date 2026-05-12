using StatHammer.Server.Simulation.Battle.Models.Aggregation;

namespace StatHammer.Server.Simulation.Battle.Services
{
    public interface IBattleBatchSimulationService
    {
        Task<BattleSimulationBatchResult> RunBatchAsync(
            int unitAId,
            int unitBId,
            int simulationCount,
            int maxTurns,
            bool unitAPrefersMelee,
            bool unitBPrefersMelee,
            CancellationToken cancellationToken = default);
    }
}
