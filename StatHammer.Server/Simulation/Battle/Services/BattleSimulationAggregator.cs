using StatHammer.Server.Simulation.Battle.Models;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;

namespace StatHammer.Server.Simulation.Battle.Services
{
    public static class BattleSimulationAggregator
    {
        public static BattleSimulationBatchResult Aggregate(
            List<BattleSimulationResult> simulations,
            int simulationCount,
            int maxTurns)
        {
            var first = simulations.First();

            var result = new BattleSimulationBatchResult
            {
                UnitAName = first.UnitAName,
                UnitBName = first.UnitBName,
                SimulationCount = simulationCount,
                MaxTurns = maxTurns,
                UnitAWins = simulations.Count(s => s.Outcome == "UnitA"),
                UnitBWins = simulations.Count(s => s.Outcome == "UnitB"),
                Draws = simulations.Count(s => s.Outcome == "Draw"),
                AverageCompletedTurns = simulations.Average(s => s.CompletedTurns),
                AverageUnitAFinalAliveModels = simulations.Average(s => s.UnitAFinalAliveModels),
                AverageUnitAFinalTotalWounds = simulations.Average(s => s.UnitAFinalTotalWounds),
                AverageUnitBFinalAliveModels = simulations.Average(s => s.UnitBFinalAliveModels),
                AverageUnitBFinalTotalWounds = simulations.Average(s => s.UnitBFinalTotalWounds)
            };

            for (int turnNumber = 1; turnNumber <= maxTurns; turnNumber++)
            {
                var turnSimulations = simulations
                    .Where(s => s.Turns.Any(t => t.TurnNumber == turnNumber))
                    .ToList();

                if (!turnSimulations.Any())
                {
                    continue;
                }

                var turnResult = new AverageBattleTurnStat
                {
                    TurnNumber = turnNumber
                };

                var sideATurns = turnSimulations
                    .Select(s => s.Turns.FirstOrDefault(t => t.TurnNumber == turnNumber)?.SideAAction)
                    .Where(x => x != null)
                    .Cast<BattleSideTurnResult>()
                    .ToList();

                if (sideATurns.Any())
                {
                    turnResult.SideA = AggregateSideTurn("A", sideATurns);
                }

                var sideBTurns = turnSimulations
                    .Select(s => s.Turns.FirstOrDefault(t => t.TurnNumber == turnNumber)?.SideBAction)
                    .Where(x => x != null)
                    .Cast<BattleSideTurnResult>()
                    .ToList();

                if (sideBTurns.Any())
                {
                    turnResult.SideB = AggregateSideTurn("B", sideBTurns);
                }

                result.Turns.Add(turnResult);
            }

            return result;
        }

        private static AverageSideTurnStat AggregateSideTurn(
            string side,
            List<BattleSideTurnResult> turns)
        {
            var result = new AverageSideTurnStat
            {
                Side = side,
                AverageStartingTargetAliveModels = turns.Average(t => t.StartingTargetAliveModels),
                AverageStartingTargetTotalWounds = turns.Average(t => t.StartingTargetTotalWounds),
                AverageEndingTargetAliveModels = turns.Average(t => t.EndingTargetAliveModels),
                AverageEndingTargetTotalWounds = turns.Average(t => t.EndingTargetTotalWounds),
                AverageAttacks = turns.Average(t => t.TotalAttacks),
                AverageHits = turns.Average(t => t.TotalHits),
                AverageWounds = turns.Average(t => t.TotalWounds),
                AverageSuccessfulSaves = turns.Average(t => t.TotalSuccessfulSaves),
                AverageDamageBeforeFnp = turns.Average(t => t.TotalDamageBeforeFnp),
                AverageBlockedByFnp = turns.Average(t => t.TotalBlockedByFnp),
                AverageFinalDamage = turns.Average(t => t.TotalFinalDamage),
                AverageModelsKilled = turns.Average(t => t.ModelsKilled)
            };

            var groupedWeaponStats = turns
                .SelectMany(t => t.WeaponResults)
                .GroupBy(w => new { w.WeaponName, w.WeaponProfileName });

            foreach (var group in groupedWeaponStats)
            {
                result.WeaponStats.Add(new AverageWeaponTurnStat
                {
                    WeaponName = group.Key.WeaponName,
                    WeaponProfileName = group.Key.WeaponProfileName,
                    AverageCount = group.Average(x => x.Count),
                    AverageAttacks = group.Average(x => x.TotalAttacks),
                    AverageHits = group.Average(x => x.TotalHits),
                    AverageWounds = group.Average(x => x.TotalWounds),
                    AverageSuccessfulSaves = group.Average(x => x.TotalSuccessfulSaves),
                    AverageDamageBeforeFnp = group.Average(x => x.TotalDamageBeforeFnp),
                    AverageBlockedByFnp = group.Average(x => x.TotalBlockedByFnp),
                    AverageFinalDamage = group.Average(x => x.TotalFinalDamage)
                });
            }

            result.WeaponStats = result.WeaponStats
                .OrderBy(w => w.WeaponName)
                .ThenBy(w => w.WeaponProfileName)
                .ToList();

            return result;
        }
    }
}
