using StatHammer.Server.Simulation.Battle.Models;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;
using StatHammer.Server.Simulation.Services;

namespace StatHammer.Server.Simulation.Battle.Services
{
    public class BattleBatchSimulationParallelService : IBattleBatchSimulationParallelService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BattleBatchSimulationParallelService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<BattleSimulationBatchResult> RunBatchAsync(
            int unitAId,
            int unitBId,
            int simulationCount,
            int maxTurns,
            bool unitAPrefersMelee,
            bool unitBPrefersMelee,
            int maxDegreeOfParallelism,
            CancellationToken cancellationToken = default)
        {
            if (simulationCount <= 0)
            {
                throw new ArgumentException("Simulation count must be greater than zero.", nameof(simulationCount));
            }

            if (maxDegreeOfParallelism <= 0)
            {
                throw new ArgumentException("MaxDegreeOfParallelism must be greater than zero.", nameof(maxDegreeOfParallelism));
            }

            var workerCount = Math.Min(maxDegreeOfParallelism, simulationCount);
            var simulationsPerWorker = simulationCount / workerCount;
            var remainder = simulationCount % workerCount;

            var tasks = new List<Task<List<BattleSimulationResult>>>();

            for (int workerIndex = 0; workerIndex < workerCount; workerIndex++)
            {
                var workerSimulationCount = simulationsPerWorker + (workerIndex < remainder ? 1 : 0);

                tasks.Add(Task.Run(async () =>
                {
                    using var scope = _scopeFactory.CreateScope();

                    var unitRuntimeBuilder = scope.ServiceProvider.GetRequiredService<IUnitRuntimeBuilder>();
                    var battleSimulationService = scope.ServiceProvider.GetRequiredService<IBattleSimulationService>();

                    var localResults = new List<BattleSimulationResult>(workerSimulationCount);

                    for (int i = 0; i < workerSimulationCount; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var unitA = await unitRuntimeBuilder.BuildUnitAsync(
                            unitAId,
                            unitAPrefersMelee,
                            cancellationToken);

                        if (unitA == null)
                        {
                            throw new InvalidOperationException($"UnitA {unitAId} not found.");
                        }

                        var unitB = await unitRuntimeBuilder.BuildUnitAsync(
                            unitBId,
                            unitBPrefersMelee,
                            cancellationToken);

                        if (unitB == null)
                        {
                            throw new InvalidOperationException($"UnitB {unitBId} not found.");
                        }

                        var simulation = battleSimulationService.SimulateBattle(unitA, unitB, maxTurns);
                        localResults.Add(simulation);
                    }

                    return localResults;
                }, cancellationToken));
            }

            var workerResults = await Task.WhenAll(tasks);
            var allSimulations = workerResults.SelectMany(r => r).ToList();

            return BattleSimulationAggregator.Aggregate(allSimulations, simulationCount, maxTurns);
        }
    }
}
