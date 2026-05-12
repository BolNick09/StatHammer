using StatHammer.Server.Simulation.Battle.Models;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;
using StatHammer.Server.Simulation.Services;

namespace StatHammer.Server.Simulation.Battle.Services
{
    public class BattleBatchSimulationService : IBattleBatchSimulationService
    {
        private readonly IUnitRuntimeBuilder _unitRuntimeBuilder;
        private readonly IBattleSimulationService _battleSimulationService;

        public BattleBatchSimulationService(
            IUnitRuntimeBuilder unitRuntimeBuilder,
            IBattleSimulationService battleSimulationService)
        {
            _unitRuntimeBuilder = unitRuntimeBuilder;
            _battleSimulationService = battleSimulationService;
        }

        public async Task<BattleSimulationBatchResult> RunBatchAsync(
            int unitAId,
            int unitBId,
            int simulationCount,
            int maxTurns,
            bool unitAPrefersMelee,
            bool unitBPrefersMelee,
            CancellationToken cancellationToken = default)
        {
            var simulations = new List<BattleSimulationResult>();

            for (int i = 0; i < simulationCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var unitA = await _unitRuntimeBuilder.BuildUnitAsync(unitAId, unitAPrefersMelee, cancellationToken);
                if (unitA == null)
                {
                    throw new InvalidOperationException($"UnitA {unitAId} not found.");
                }

                var unitB = await _unitRuntimeBuilder.BuildUnitAsync(unitBId, unitBPrefersMelee, cancellationToken);
                if (unitB == null)
                {
                    throw new InvalidOperationException($"UnitB {unitBId} not found.");
                }

                var simulation = _battleSimulationService.SimulateBattle(unitA, unitB, maxTurns);
                simulations.Add(simulation);
            }

            return BattleSimulationAggregator.Aggregate(simulations, simulationCount, maxTurns);
        } 
    }
}
