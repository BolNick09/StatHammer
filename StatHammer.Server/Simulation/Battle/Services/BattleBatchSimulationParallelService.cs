using StatHammer.Server.Simulation.Battle.Models;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;
using StatHammer.Server.Simulation.Models;
using StatHammer.Server.Simulation.Runtime;
using StatHammer.Server.Simulation.Services;
using System.Diagnostics;

namespace StatHammer.Server.Simulation.Battle.Services
{
    public class BattleBatchSimulationParallelService
        : IBattleBatchSimulationParallelService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BattleBatchSimulationParallelService> _logger;

        public BattleBatchSimulationParallelService(
            IServiceScopeFactory scopeFactory,
            ILogger<BattleBatchSimulationParallelService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<BattleSimulationBatchResult> RunBatchAsync(
            int unitAId,
            int unitBId,
            int simulationCount,
            int maxTurns,
            bool unitAPrefersMelee,
            bool unitBPrefersMelee,
            int maxDegreeOfParallelism,
            CancellationToken cancellationToken = default,
            SimulationModifiers? modifiers = null)
        {
            if (simulationCount <= 0)
            {
                throw new ArgumentException(
                    "Simulation count must be greater than zero.",
                    nameof(simulationCount));
            }

            if (maxDegreeOfParallelism <= 0)
            {
                throw new ArgumentException(
                    "MaxDegreeOfParallelism must be greater than zero.",
                    nameof(maxDegreeOfParallelism));
            }

            var totalStopwatch = Stopwatch.StartNew();

            var prototypeBuildStopwatch = Stopwatch.StartNew();

            SimulationUnit prototypeUnitA;
            SimulationUnit prototypeUnitB;

            using (var prototypeScope = _scopeFactory.CreateScope())
            {
                var unitRuntimeBuilder =
                    prototypeScope.ServiceProvider.GetRequiredService<IUnitRuntimeBuilder>();

                var builtUnitA = await unitRuntimeBuilder.BuildUnitAsync(
                    unitAId,
                    unitAPrefersMelee,
                    cancellationToken);

                if (builtUnitA == null)
                {
                    throw new InvalidOperationException($"UnitA {unitAId} not found.");
                }

                var builtUnitB = await unitRuntimeBuilder.BuildUnitAsync(
                    unitBId,
                    unitBPrefersMelee,
                    cancellationToken);

                if (builtUnitB == null)
                {
                    throw new InvalidOperationException($"UnitB {unitBId} not found.");
                }

                prototypeUnitA = builtUnitA;
                prototypeUnitB = builtUnitB;
            }

            prototypeBuildStopwatch.Stop();

            var workerCount = Math.Min(maxDegreeOfParallelism, simulationCount);
            var simulationsPerWorker = simulationCount / workerCount;
            var remainder = simulationCount % workerCount;

            _logger.LogInformation(
                "Parallel batch started. UnitAId={UnitAId}, UnitBId={UnitBId}, " +
                "SimulationCount={SimulationCount}, MaxTurns={MaxTurns}, " +
                "RequestedParallelism={RequestedParallelism}, WorkerCount={WorkerCount}, " +
                "PrototypeBuildElapsedMs={PrototypeBuildElapsedMs}",
                unitAId,
                unitBId,
                simulationCount,
                maxTurns,
                maxDegreeOfParallelism,
                workerCount,
                prototypeBuildStopwatch.Elapsed.TotalMilliseconds);

            var tasks = new List<Task<List<BattleSimulationResult>>>();

            for (int workerIndex = 0; workerIndex < workerCount; workerIndex++)
            {
                var capturedWorkerIndex = workerIndex;

                var workerSimulationCount =
                    simulationsPerWorker +
                    (workerIndex < remainder ? 1 : 0);

                tasks.Add(Task.Run(() =>
                {
                    var workerStopwatch = Stopwatch.StartNew();
                    var cloneUnitsElapsed = TimeSpan.Zero;
                    var simulateBattlesElapsed = TimeSpan.Zero;

                    _logger.LogInformation(
                        "Worker {WorkerIndex} started. SimulationCount={SimulationCount}",
                        capturedWorkerIndex,
                        workerSimulationCount);

                    using var scope = _scopeFactory.CreateScope();

                    var battleSimulationService =
                        scope.ServiceProvider.GetRequiredService<IBattleSimulationService>();

                    var localResults =
                        new List<BattleSimulationResult>(workerSimulationCount);

                    for (int i = 0; i < workerSimulationCount; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var cloneStopwatch = Stopwatch.StartNew();

                        var unitA = prototypeUnitA.DeepClone();
                        var unitB = prototypeUnitB.DeepClone();

                        cloneStopwatch.Stop();
                        cloneUnitsElapsed += cloneStopwatch.Elapsed;

                        var simulationStopwatch = Stopwatch.StartNew();

                        var simulation = battleSimulationService.SimulateBattle(
                            unitA,
                            unitB,
                            maxTurns,
                            modifiers);

                        simulationStopwatch.Stop();
                        simulateBattlesElapsed += simulationStopwatch.Elapsed;

                        localResults.Add(simulation);
                    }

                    workerStopwatch.Stop();

                    _logger.LogInformation(
                        "Worker {WorkerIndex} completed. " +
                        "SimulationCount={SimulationCount}, " +
                        "TotalElapsedMs={TotalElapsedMs}, " +
                        "CloneUnitsElapsedMs={CloneUnitsElapsedMs}, " +
                        "SimulateBattlesElapsedMs={SimulateBattlesElapsedMs}",
                        capturedWorkerIndex,
                        workerSimulationCount,
                        workerStopwatch.ElapsedMilliseconds,
                        cloneUnitsElapsed.TotalMilliseconds,
                        simulateBattlesElapsed.TotalMilliseconds);

                    return localResults;
                }, cancellationToken));
            }

            var workerResults = await Task.WhenAll(tasks);

            var aggregationStopwatch = Stopwatch.StartNew();

            var allSimulations = workerResults
                .SelectMany(results => results)
                .ToList();

            var result = BattleSimulationAggregator.Aggregate(
                allSimulations,
                simulationCount,
                maxTurns);

            aggregationStopwatch.Stop();
            totalStopwatch.Stop();

            _logger.LogInformation(
                "Parallel batch completed. " +
                "SimulationCount={SimulationCount}, " +
                "WorkerCount={WorkerCount}, " +
                "PrototypeBuildElapsedMs={PrototypeBuildElapsedMs}, " +
                "AggregationElapsedMs={AggregationElapsedMs}, " +
                "TotalElapsedMs={TotalElapsedMs}",
                simulationCount,
                workerCount,
                prototypeBuildStopwatch.Elapsed.TotalMilliseconds,
                aggregationStopwatch.ElapsedMilliseconds,
                totalStopwatch.ElapsedMilliseconds);

            return result;
        }
    }
}