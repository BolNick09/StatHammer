using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;

namespace StatHammer.Server.PageServices.Admin.SimulationResults
{
    public class SimulationResultAdminPageService : ISimulationResultAdminPageService
    {
        private readonly StatHammerDbContext _context;

        public SimulationResultAdminPageService(StatHammerDbContext context)
        {
            _context = context;
        }

        public async Task<List<SimulationResultListItemViewModel>> GetResultsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SimulationResults
                .Include(sr => sr.UnitA)
                .Include(sr => sr.UnitB)
                .Include(sr => sr.TurnStats)
                .OrderByDescending(sr => sr.CreatedAtUtc)
                .Select(sr => new SimulationResultListItemViewModel
                {
                    Id = sr.Id,
                    UnitAId = sr.UnitAId,
                    UnitAName = sr.UnitA != null ? sr.UnitA.Name : string.Empty,
                    UnitBId = sr.UnitBId,
                    UnitBName = sr.UnitB != null ? sr.UnitB.Name : string.Empty,
                    SimulationCount = sr.SimulationCount,
                    CreatedAtUtc = sr.CreatedAtUtc,
                    TurnStatsCount = sr.TurnStats.Count
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<SimulationResultDetailsViewModel?> GetResultDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = await _context.SimulationResults
                .Include(sr => sr.UnitA)
                .Include(sr => sr.UnitB)
                .Include(sr => sr.TurnStats)
                    .ThenInclude(ts => ts.WeaponStats)
                        .ThenInclude(ws => ws.Weapon)
                .FirstOrDefaultAsync(sr => sr.Id == id, cancellationToken);

            if (result == null)
            {
                return null;
            }

            return new SimulationResultDetailsViewModel
            {
                Id = result.Id,
                UnitAId = result.UnitAId,
                UnitAName = result.UnitA?.Name ?? string.Empty,
                UnitBId = result.UnitBId,
                UnitBName = result.UnitB?.Name ?? string.Empty,
                SimulationCount = result.SimulationCount,
                CreatedAtUtc = result.CreatedAtUtc,
                TurnStats = result.TurnStats
                    .OrderBy(ts => ts.TurnNumber)
                    .ThenBy(ts => ts.Side)
                    .Select(ts => new SimulationResultTurnStatViewModel
                    {
                        Id = ts.Id,
                        TurnNumber = ts.TurnNumber,
                        Side = ts.Side,
                        AvgModelsAlive = ts.AvgModelsAlive,
                        AvgWoundsAlive = ts.AvgWoundsAlive,
                        AvgHits = ts.AvgHits,
                        AvgWounds = ts.AvgWounds,
                        AvgSuccessfulSaves = ts.AvgSuccessfulSaves,
                        AvgBlockedByFnp = ts.AvgBlockedByFnp,
                        WeaponStats = ts.WeaponStats
                            .OrderBy(ws => ws.Weapon != null ? ws.Weapon.Name : string.Empty)
                            .Select(ws => new SimulationResultWeaponStatViewModel
                            {
                                Id = ws.Id,
                                WeaponId = ws.WeaponId,
                                WeaponName = ws.Weapon != null ? ws.Weapon.Name : string.Empty,
                                AvgHits = ws.AvgHits,
                                AvgWounds = ws.AvgWounds,
                                AvgDamage = ws.AvgDamage
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }

        public async Task<bool> DeleteResultAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = await _context.SimulationResults
                .Include(sr => sr.TurnStats)
                    .ThenInclude(ts => ts.WeaponStats)
                .FirstOrDefaultAsync(sr => sr.Id == id, cancellationToken);

            if (result == null)
            {
                return false;
            }

            _context.SimulationResults.Remove(result);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}