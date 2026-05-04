using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
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
        public async Task<ActionResult<IEnumerable<Unit>>> GetUnits()
        {
            var units = await _context.Units.ToListAsync();
            return Ok(units);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Unit>> GetUnit(int id)
        {
            var unit = await _context.Units.FindAsync(id);

            if (unit == null)            
                return NotFound();
            

            return Ok(unit);
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> CreateUnit(Unit unit)
        {
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUnit), new { id = unit.Id }, unit);
        }
    }
}
