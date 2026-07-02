using StatHammer.Server.Simulation.Battle.Models.Aggregation;
using StatHammer.Server.Simulation.Models;

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
            CancellationToken cancellationToken = default,
            SimulationModifiers? modifiers = null,
            SimulationLoadout? loadout = null);
    }
}