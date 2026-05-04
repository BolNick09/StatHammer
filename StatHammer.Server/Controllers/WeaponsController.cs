using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
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
        public async Task<ActionResult<IEnumerable<Weapon>>> GetWeapons()
        {
            var weapons = await _context.Weapons
                .Include(w => w.WeaponProfiles)
                .ToListAsync();

            return Ok(weapons);
        }

        [HttpPost]
        public async Task<ActionResult<Weapon>> CreateWeapon(Weapon weapon)
        {
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWeapons), new { id = weapon.Id }, weapon);
        }
    }
}
