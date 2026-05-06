using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Simulation.Services;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationDebugController : ControllerBase
    {
        private readonly IUnitRuntimeBuilder _unitRuntimeBuilder;

        public SimulationDebugController(IUnitRuntimeBuilder unitRuntimeBuilder)
        {
            _unitRuntimeBuilder = unitRuntimeBuilder;
        }

        [HttpGet("unit/{unitId}")]
        public async Task<IActionResult> BuildUnit(int unitId, [FromQuery] bool preferMelee = false)
        {
            var unit = await _unitRuntimeBuilder.BuildUnitAsync(unitId, preferMelee);

            if (unit == null)
            {
                return NotFound();
            }

            return Ok(unit);
        }
    }
}
