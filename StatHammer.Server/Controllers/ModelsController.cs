using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
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
        public async Task<ActionResult<IEnumerable<Model>>> GetModels()
        {
            var models = await _context.Models
                .Include(m => m.ModelWeapons)
                    .ThenInclude(mw => mw.Weapon)
                .Include(m => m.ModelWargears)
                    .ThenInclude(mw => mw.Wargear)
                .Include(m => m.ModelAbilities)
                    .ThenInclude(ma => ma.Ability)
                .ToListAsync();

            return Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Model>> GetModel(int id)
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

            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<Model>> CreateModel(Model model)
        {
            _context.Models.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetModel), new { id = model.Id }, model);
        }
    }
}
