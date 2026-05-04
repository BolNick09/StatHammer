using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Mappings;
using StatHammer.Server.Models.DTOs.Simulations;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationResultsController : ControllerBase
    {
        private readonly StatHammerDbContext _context;

        public SimulationResultsController(StatHammerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SimulationResultReadDto>>> GetSimulationResults()
        {
            var results = await _context.SimulationResults
                .Include(sr => sr.UnitA)
                .Include(sr => sr.UnitB)
                .Include(sr => sr.TurnStats)
                    .ThenInclude(ts => ts.WeaponStats)
                        .ThenInclude(ws => ws.Weapon)
                .OrderByDescending(sr => sr.CreatedAtUtc)
                .ToListAsync();

            return Ok(results.Select(r => r.ToReadDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SimulationResultReadDto>> GetSimulationResult(int id)
        {
            var result = await _context.SimulationResults
                .Include(sr => sr.UnitA)
                .Include(sr => sr.UnitB)
                .Include(sr => sr.TurnStats)
                    .ThenInclude(ts => ts.WeaponStats)
                        .ThenInclude(ws => ws.Weapon)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.ToReadDto());
        }

        [HttpPost]
        public async Task<ActionResult<SimulationResultReadDto>> CreateSimulationResult(
            SimulationResultCreateDto dto)
        {
            var unitAExists = await _context.Units.AnyAsync(u => u.Id == dto.UnitAId);
            var unitBExists = await _context.Units.AnyAsync(u => u.Id == dto.UnitBId);

            if (!unitAExists)
            {
                return BadRequest($"UnitAId {dto.UnitAId} does not exist.");
            }

            if (!unitBExists)
            {
                return BadRequest($"UnitBId {dto.UnitBId} does not exist.");
            }

            if (dto.SimulationCount <= 0)
            {
                return BadRequest("SimulationCount must be greater than zero.");
            }

            if (dto.TurnStats.Any(ts => ts.TurnNumber < 1 || ts.TurnNumber > 5))
            {
                return BadRequest("TurnNumber must be between 1 and 5.");
            }

            if (dto.TurnStats.Any(ts => ts.Side != "A" && ts.Side != "B"))
            {
                return BadRequest("Side must be either 'A' or 'B'.");
            }

            var weaponIds = dto.TurnStats
                .SelectMany(ts => ts.WeaponStats)
                .Select(ws => ws.WeaponId)
                .Distinct()
                .ToList();

            var existingWeaponIds = await _context.Weapons
                .Where(w => weaponIds.Contains(w.Id))
                .Select(w => w.Id)
                .ToListAsync();

            var missingWeaponIds = weaponIds
                .Except(existingWeaponIds)
                .ToList();

            if (missingWeaponIds.Any())
            {
                return BadRequest($"Weapons not found: {string.Join(", ", missingWeaponIds)}.");
            }

            var result = dto.ToEntity();

            _context.SimulationResults.Add(result);
            await _context.SaveChangesAsync();

            var createdResult = await _context.SimulationResults
                .Include(sr => sr.UnitA)
                .Include(sr => sr.UnitB)
                .Include(sr => sr.TurnStats)
                    .ThenInclude(ts => ts.WeaponStats)
                        .ThenInclude(ws => ws.Weapon)
                .FirstAsync(sr => sr.Id == result.Id);

            return CreatedAtAction(
                nameof(GetSimulationResult),
                new { id = createdResult.Id },
                createdResult.ToReadDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSimulationResult(int id)
        {
            var result = await _context.SimulationResults.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            _context.SimulationResults.Remove(result);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
