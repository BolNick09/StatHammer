using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Mappings;
using StatHammer.Server.Models.DTOs.UnitModels;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitModelsController : ControllerBase
    {
        private readonly StatHammerDbContext _context;

        public UnitModelsController(StatHammerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitModelLinkReadDto>>> GetUnitModels()
        {
            var unitModels = await _context.UnitModels
                .Include(um => um.Unit)
                .Include(um => um.Model)
                .ToListAsync();

            return Ok(unitModels.Select(um => um.ToReadDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UnitModelLinkReadDto>> GetUnitModel(int id)
        {
            var unitModel = await _context.UnitModels
                .Include(um => um.Unit)
                .Include(um => um.Model)
                .FirstOrDefaultAsync(um => um.Id == id);

            if (unitModel == null)
            {
                return NotFound();
            }

            return Ok(unitModel.ToReadDto());
        }

        [HttpPost]
        public async Task<ActionResult<UnitModelLinkReadDto>> CreateUnitModel(UnitModelLinkCreateDto dto)
        {
            if (dto.MinCount < 0)
            {
                return BadRequest("MinCount must be greater than or equal to 0.");
            }

            if (dto.MaxCount < 1)
            {
                return BadRequest("MaxCount must be greater than 0.");
            }

            if (dto.MinCount > dto.MaxCount)
            {
                return BadRequest("MinCount cannot be greater than MaxCount.");
            }

            var unitExists = await _context.Units.AnyAsync(u => u.Id == dto.UnitId);
            if (!unitExists)
            {
                return BadRequest($"UnitId {dto.UnitId} does not exist.");
            }

            var modelExists = await _context.Models.AnyAsync(m => m.Id == dto.ModelId);
            if (!modelExists)
            {
                return BadRequest($"ModelId {dto.ModelId} does not exist.");
            }

            var duplicateExists = await _context.UnitModels
                .AnyAsync(um => um.UnitId == dto.UnitId && um.ModelId == dto.ModelId);

            if (duplicateExists)
            {
                return BadRequest($"A UnitModel with UnitId {dto.UnitId} and ModelId {dto.ModelId} already exists.");
            }

            var unitModel = dto.ToEntity();

            _context.UnitModels.Add(unitModel);
            await _context.SaveChangesAsync();

            var createdUnitModel = await _context.UnitModels
                .Include(um => um.Unit)
                .Include(um => um.Model)
                .FirstAsync(um => um.Id == unitModel.Id);

            return CreatedAtAction(nameof(GetUnitModel), new { id = createdUnitModel.Id }, createdUnitModel.ToReadDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnitModel(int id)
        {
            var unitModel = await _context.UnitModels.FindAsync(id);

            if (unitModel == null)
            {
                return NotFound();
            }

            _context.UnitModels.Remove(unitModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
