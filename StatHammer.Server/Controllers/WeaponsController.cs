using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Mappings;
using StatHammer.Server.Models.DTOs.Weapons;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeaponsController : ControllerBase
    {
        private readonly StatHammerDbContext _context;

        public WeaponsController(StatHammerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeaponReadDto>>> GetWeapons()
        {
            var weapons = await _context.Weapons
                .Include(w => w.WeaponProfiles)
                    .ThenInclude(wp => wp.WeaponProfileAbilities)
                        .ThenInclude(wpa => wpa.Ability)
                .ToListAsync();

            return Ok(weapons.Select(w => w.ToReadDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeaponReadDto>> GetWeapon(int id)
        {
            var weapon = await _context.Weapons
                .Include(w => w.WeaponProfiles)
                    .ThenInclude(wp => wp.WeaponProfileAbilities)
                        .ThenInclude(wpa => wpa.Ability)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (weapon == null)
            {
                return NotFound();
            }

            return Ok(weapon.ToReadDto());
        }

        [HttpPost]
        public async Task<ActionResult<WeaponReadDto>> CreateWeapon(WeaponCreateDto dto)
        {
            var weapon = dto.ToEntity();

            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();

            var createdWeapon = await _context.Weapons
                .Include(w => w.WeaponProfiles)
                    .ThenInclude(wp => wp.WeaponProfileAbilities)
                        .ThenInclude(wpa => wpa.Ability)
                .FirstAsync(w => w.Id == weapon.Id);

            return CreatedAtAction(nameof(GetWeapon), new { id = createdWeapon.Id }, createdWeapon.ToReadDto());
        }
    }
}
