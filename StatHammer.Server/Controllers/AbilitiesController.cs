using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbilitiesController : ControllerBase
    {
        private readonly StatHammerDbContext _context;

        public AbilitiesController(StatHammerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ability>>> GetAbilities()
        {
            return Ok(await _context.Abilities.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Ability>> CreateAbility(Ability ability)
        {
            _context.Abilities.Add(ability);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAbilities), new { id = ability.Id }, ability);
        }
    }
}
