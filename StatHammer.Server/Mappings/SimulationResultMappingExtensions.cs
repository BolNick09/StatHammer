using StatHammer.Server.Models.DTOs.Simulations;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Mappings
{
    public static class SimulationResultMappingExtensions
    {
        public static SimulationResultReadDto ToReadDto(this SimulationResult result)
        {
            return new SimulationResultReadDto
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
                    .Select(ts => new TurnStatReadDto
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
                            .OrderBy(ws => ws.WeaponId)
                            .Select(ws => new WeaponStatReadDto
                            {
                                Id = ws.Id,
                                WeaponId = ws.WeaponId,
                                WeaponName = ws.Weapon?.Name ?? string.Empty,
                                AvgHits = ws.AvgHits,
                                AvgWounds = ws.AvgWounds,
                                AvgDamage = ws.AvgDamage
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }

        public static SimulationResult ToEntity(this SimulationResultCreateDto dto)
        {
            return new SimulationResult
            {
                UnitAId = dto.UnitAId,
                UnitBId = dto.UnitBId,
                SimulationCount = dto.SimulationCount,
                CreatedAtUtc = DateTime.UtcNow,
                TurnStats = dto.TurnStats
                    .Select(ts => new TurnStat
                    {
                        TurnNumber = ts.TurnNumber,
                        Side = ts.Side,
                        AvgModelsAlive = ts.AvgModelsAlive,
                        AvgWoundsAlive = ts.AvgWoundsAlive,
                        AvgHits = ts.AvgHits,
                        AvgWounds = ts.AvgWounds,
                        AvgSuccessfulSaves = ts.AvgSuccessfulSaves,
                        AvgBlockedByFnp = ts.AvgBlockedByFnp,
                        WeaponStats = ts.WeaponStats
                            .Select(ws => new WeaponStat
                            {
                                WeaponId = ws.WeaponId,
                                AvgHits = ws.AvgHits,
                                AvgWounds = ws.AvgWounds,
                                AvgDamage = ws.AvgDamage
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }
}
