using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Mappings;
using StatHammer.Server.Models.DTOs.Models;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelsController : ControllerBase
    {
        private readonly StatHammerDbContext _context;

        public ModelsController(StatHammerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelReadDto>>> GetModels()
        {
            var models = await _context.Models
                .Include(m => m.ModelWeapons)
                    .ThenInclude(mw => mw.Weapon)
                .Include(m => m.ModelWargears)
                    .ThenInclude(mw => mw.Wargear)
                .Include(m => m.ModelAbilities)
                    .ThenInclude(ma => ma.Ability)
                .ToListAsync();

            return Ok(models.Select(m => m.ToReadDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModelReadDto>> GetModel(int id)
        {
            var model = await _context.Models
                .Include(m => m.ModelWeapons)
                    .ThenInclude(mw => mw.Weapon)
                .Include(m => m.ModelWargears)
                    .ThenInclude(mw => mw.Wargear)
                .Include(m => m.ModelAbilities)
                    .ThenInclude(ma => ma.Ability)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            return Ok(model.ToReadDto());
        }

        [HttpPost]
        public async Task<ActionResult<ModelReadDto>> CreateModel(ModelCreateDto dto)
        {
            var weaponIds = dto.ModelWeapons
                .Select(w => w.WeaponId)
                .Distinct()
                .ToList();

            var existingWeaponIds = await _context.Weapons
                .Where(w => weaponIds.Contains(w.Id))
                .Select(w => w.Id)
                .ToListAsync();

            var missingWeaponIds = weaponIds.Except(existingWeaponIds).ToList();
            if (missingWeaponIds.Any())
            {
                return BadRequest($"Weapons not found: {string.Join(", ", missingWeaponIds)}.");
            }

            var wargearIds = dto.ModelWargears
                .Select(w => w.WargearId)
                .Distinct()
                .ToList();

            if (wargearIds.Any())
            {
                var existingWargearIds = await _context.Wargears
                    .Where(w => wargearIds.Contains(w.Id))
                    .Select(w => w.Id)
                    .ToListAsync();

                var missingWargearIds = wargearIds.Except(existingWargearIds).ToList();
                if (missingWargearIds.Any())
                {
                    return BadRequest($"Wargears not found: {string.Join(", ", missingWargearIds)}.");
                }
            }

            var abilityIds = dto.AbilityIds
                .Distinct()
                .ToList();

            if (abilityIds.Any())
            {
                var existingAbilityIds = await _context.Abilities
                    .Where(a => abilityIds.Contains(a.Id))
                    .Select(a => a.Id)
                    .ToListAsync();

                var missingAbilityIds = abilityIds.Except(existingAbilityIds).ToList();
                if (missingAbilityIds.Any())
                {
                    return BadRequest($"Abilities not found: {string.Join(", ", missingAbilityIds)}.");
                }
            }

            var model = dto.ToEntity();

            _context.Models.Add(model);
            await _context.SaveChangesAsync();

            var createdModel = await _context.Models
                .Include(m => m.ModelWeapons)
                    .ThenInclude(mw => mw.Weapon)
                .Include(m => m.ModelWargears)
                    .ThenInclude(mw => mw.Wargear)
                .Include(m => m.ModelAbilities)
                    .ThenInclude(ma => ma.Ability)
                .FirstAsync(m => m.Id == model.Id);

            return CreatedAtAction(nameof(GetModel), new { id = createdModel.Id }, createdModel.ToReadDto());
        }
    }
}
