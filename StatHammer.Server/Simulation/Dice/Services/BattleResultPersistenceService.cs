using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Models.Entities;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;

namespace StatHammer.Server.Simulation.Dice.Services
{
    public class BattleResultPersistenceService : IBattleResultPersistenceService
    {
        private readonly StatHammerDbContext _context;

        public BattleResultPersistenceService(StatHammerDbContext context)
        {
            _context = context;
        }

        public async Task<SimulationResult> SaveBatchResultAsync(
            int unitAId,
            int unitBId,
            BattleSimulationBatchResult batchResult,
            CancellationToken cancellationToken = default)
        {
            var unitAExists = await _context.Units.AnyAsync(u => u.Id == unitAId, cancellationToken);
            if (!unitAExists)
            {
                throw new InvalidOperationException($"UnitA {unitAId} not found.");
            }

            var unitBExists = await _context.Units.AnyAsync(u => u.Id == unitBId, cancellationToken);
            if (!unitBExists)
            {
                throw new InvalidOperationException($"UnitB {unitBId} not found.");
            }

            var allWeaponIds = batchResult.Turns
                .SelectMany(t => new[] { t.SideA, t.SideB })
                .Where(side => side != null)
                .SelectMany(side => side!.WeaponStats)
                .Select(ws => ws.WeaponName)
                .Distinct()
                .ToList();

            var weaponLookup = await _context.Weapons
                .ToDictionaryAsync(w => w.Name, w => w.Id, cancellationToken);

            var simulationResult = new SimulationResult
            {
                UnitAId = unitAId,
                UnitBId = unitBId,
                SimulationCount = batchResult.SimulationCount,
                CreatedAtUtc = DateTime.UtcNow,
                TurnStats = new List<TurnStat>()
            };

            foreach (var turn in batchResult.Turns)
            {
                if (turn.SideA != null)
                {
                    simulationResult.TurnStats.Add(CreateTurnStat(turn.TurnNumber, turn.SideA, weaponLookup));
                }

                if (turn.SideB != null)
                {
                    simulationResult.TurnStats.Add(CreateTurnStat(turn.TurnNumber, turn.SideB, weaponLookup));
                }
            }

            _context.SimulationResults.Add(simulationResult);
            await _context.SaveChangesAsync(cancellationToken);

            return simulationResult;
        }

        private static TurnStat CreateTurnStat(
            int turnNumber,
            AverageSideTurnStat sideStat,
            Dictionary<string, int> weaponLookup)
        {
            var turnStat = new TurnStat
            {
                TurnNumber = turnNumber,
                Side = sideStat.Side,
                AvgModelsAlive = sideStat.AverageEndingTargetAliveModels,
                AvgWoundsAlive = sideStat.AverageEndingTargetTotalWounds,
                AvgHits = sideStat.AverageHits,
                AvgWounds = sideStat.AverageWounds,
                AvgSuccessfulSaves = sideStat.AverageSuccessfulSaves,
                AvgBlockedByFnp = sideStat.AverageBlockedByFnp,
                WeaponStats = new List<WeaponStat>()
            };

            foreach (var weaponStat in sideStat.WeaponStats)
            {
                if (!weaponLookup.TryGetValue(weaponStat.WeaponName, out var weaponId))
                {
                    continue;
                }

                turnStat.WeaponStats.Add(new WeaponStat
                {
                    WeaponId = weaponId,
                    AvgHits = weaponStat.AverageHits,
                    AvgWounds = weaponStat.AverageWounds,
                    AvgDamage = weaponStat.AverageFinalDamage
                });
            }

            return turnStat;
        }
    }
}
