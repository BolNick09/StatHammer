using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Mappings;
using StatHammer.Server.Models.DTOs.Units;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly StatHammerDbContext _context;

        public UnitsController(StatHammerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitReadDto>>> GetUnits()
        {
            var units = await _context.Units
                .Include(u => u.UnitModels)
                    .ThenInclude(um => um.Model)
                .Include(u => u.UnitAbilities)
                    .ThenInclude(ua => ua.Ability)
                .Include(u => u.UnitKeywords)
                    .ThenInclude(uk => uk.Keyword)
                .Include(u => u.UnitOptions)
                    .ThenInclude(uo => uo.OptionItems)
                        .ThenInclude(oi => oi.Weapon)
                .Include(u => u.UnitOptions)
                    .ThenInclude(uo => uo.OptionItems)
                        .ThenInclude(oi => oi.Wargear)
                .ToListAsync();

            return Ok(units.Select(u => u.ToReadDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UnitReadDto>> GetUnit(int id)
        {
            var unit = await _context.Units
                .Include(u => u.UnitModels)
                    .ThenInclude(um => um.Model)
                .Include(u => u.UnitAbilities)
                    .ThenInclude(ua => ua.Ability)
                .Include(u => u.UnitKeywords)
                    .ThenInclude(uk => uk.Keyword)
                .Include(u => u.UnitOptions)
                    .ThenInclude(uo => uo.OptionItems)
                        .ThenInclude(oi => oi.Weapon)
                .Include(u => u.UnitOptions)
                    .ThenInclude(uo => uo.OptionItems)
                        .ThenInclude(oi => oi.Wargear)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (unit == null)
            {
                return NotFound();
            }

            return Ok(unit.ToReadDto());
        }

        [HttpPost]
        public async Task<ActionResult<UnitReadDto>> CreateUnit(UnitCreateDto dto)
        {
            var unit = dto.ToEntity();

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUnit), new { id = unit.Id }, unit.ToReadDto());
        }
    }
}
