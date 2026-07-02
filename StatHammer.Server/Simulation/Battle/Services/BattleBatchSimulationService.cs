using StatHammer.Server.Simulation.Battle.Models;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;
using StatHammer.Server.Simulation.Models;
using StatHammer.Server.Simulation.Runtime;
using StatHammer.Server.Simulation.Services;
using System.Diagnostics;

namespace StatHammer.Server.Simulation.Battle.Services
{
    public class BattleBatchSimulationService : IBattleBatchSimulationService
    {
        private readonly IUnitRuntimeBuilder _unitRuntimeBuilder;
        private readonly IBattleSimulationService _battleSimulationService;
        private readonly ILogger<BattleBatchSimulationService> _logger;

        public BattleBatchSimulationService(
            IUnitRuntimeBuilder unitRuntimeBuilder,
            IBattleSimulationService battleSimulationService,
            ILogger<BattleBatchSimulationService> logger)
        {
            _unitRuntimeBuilder = unitRuntimeBuilder;
            _battleSimulationService = battleSimulationService;
            _logger = logger;
        }

        public async Task<BattleSimulationBatchResult> RunBatchAsync(
            int unitAId,
            int unitBId,
            int simulationCount,
            int maxTurns,
            bool unitAPrefersMelee,
            bool unitBPrefersMelee,
            CancellationToken cancellationToken = default,
            SimulationModifiers? modifiers = null)
        {
            if (simulationCount <= 0)
            {
                throw new ArgumentException(
                    "Simulation count must be greater than zero.",
                    nameof(simulationCount));
            }

            var totalStopwatch = Stopwatch.StartNew();

            var prototypeBuildStopwatch = Stopwatch.StartNew();

            var prototypeUnitA = await _unitRuntimeBuilder.BuildUnitAsync(
                unitAId,
                unitAPrefersMelee,
                cancellationToken);

            if (prototypeUnitA == null)
            {
                throw new InvalidOperationException($"UnitA {unitAId} not found.");
            }

            var prototypeUnitB = await _unitRuntimeBuilder.BuildUnitAsync(
                unitBId,
                unitBPrefersMelee,
                cancellationToken);

            if (prototypeUnitB == null)
            {
                throw new InvalidOperationException($"UnitB {unitBId} not found.");
            }

            prototypeBuildStopwatch.Stop();

            var cloneUnitsElapsed = TimeSpan.Zero;
            var simulateBattlesElapsed = TimeSpan.Zero;

            _logger.LogInformation(
                "Single-thread batch started. UnitAId={UnitAId}, UnitBId={UnitBId}, " +
                "SimulationCount={SimulationCount}, MaxTurns={MaxTurns}",
                unitAId,
                unitBId,
                simulationCount,
                maxTurns);

            var simulations = new List<BattleSimulationResult>(simulationCount);

            for (int i = 0; i < simulationCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var cloneStopwatch = Stopwatch.StartNew();

                var unitA = prototypeUnitA.DeepClone();
                var unitB = prototypeUnitB.DeepClone();

                cloneStopwatch.Stop();
                cloneUnitsElapsed += cloneStopwatch.Elapsed;

                var simulationStopwatch = Stopwatch.StartNew();

                var simulation = _battleSimulationService.SimulateBattle(
                    unitA,
                    unitB,
                    maxTurns,
                    modifiers);

                simulationStopwatch.Stop();
                simulateBattlesElapsed += simulationStopwatch.Elapsed;

                simulations.Add(simulation);
            }

            var aggregationStopwatch = Stopwatch.StartNew();

            var result = BattleSimulationAggregator.Aggregate(
                simulations,
                simulationCount,
                maxTurns);

            aggregationStopwatch.Stop();
            totalStopwatch.Stop();

            _logger.LogInformation(
                "Single-thread batch completed. " +
                "SimulationCount={SimulationCount}, " +
                "PrototypeBuildElapsedMs={PrototypeBuildElapsedMs}, " +
                "CloneUnitsElapsedMs={CloneUnitsElapsedMs}, " +
                "SimulateBattlesElapsedMs={SimulateBattlesElapsedMs}, " +
                "AggregationElapsedMs={AggregationElapsedMs}, " +
                "TotalElapsedMs={TotalElapsedMs}",
                simulationCount,
                prototypeBuildStopwatch.Elapsed.TotalMilliseconds,
                cloneUnitsElapsed.TotalMilliseconds,
                simulateBattlesElapsed.TotalMilliseconds,
                aggregationStopwatch.ElapsedMilliseconds,
                totalStopwatch.ElapsedMilliseconds);

            return result;
        }
    }
}