using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Simulation.Battle.DTOs;
using StatHammer.Server.Simulation.Battle.Services;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BattleBatchParallelDebugController : ControllerBase
    {
        private readonly IBattleBatchSimulationParallelService _parallelService;

        public BattleBatchParallelDebugController(
            IBattleBatchSimulationParallelService parallelService)
        {
            _parallelService = parallelService;
        }

        [HttpPost("run-batch")]
        public async Task<IActionResult> RunBatch(
            RunBattleParallelBatchRequestDto dto,
            CancellationToken cancellationToken)
        {
            if (dto.SimulationCount <= 0)
            {
                return BadRequest("SimulationCount must be greater than zero.");
            }

            if (dto.MaxTurns < 1 || dto.MaxTurns > 5)
            {
                return BadRequest("MaxTurns must be between 1 and 5.");
            }

            if (dto.MaxDegreeOfParallelism <= 0)
            {
                return BadRequest("MaxDegreeOfParallelism must be greater than zero.");
            }

            var result = await _parallelService.RunBatchAsync(
                dto.UnitAId,
                dto.UnitBId,
                dto.SimulationCount,
                dto.MaxTurns,
                dto.UnitAPrefersMelee,
                dto.UnitBPrefersMelee,
                dto.MaxDegreeOfParallelism,
                cancellationToken);

            return Ok(result);
        }
    }
}
