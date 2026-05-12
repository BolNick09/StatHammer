using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Simulation.Battle.DTOs;
using StatHammer.Server.Simulation.Battle.Services;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BattleBatchDebugController : ControllerBase
    {
        private readonly IBattleBatchSimulationService _battleBatchSimulationService;

        public BattleBatchDebugController(IBattleBatchSimulationService battleBatchSimulationService)
        {
            _battleBatchSimulationService = battleBatchSimulationService;
        }

        [HttpPost("run-batch")]
        public async Task<IActionResult> RunBatch(
            RunBattleBatchRequestDto dto,
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

            var result = await _battleBatchSimulationService.RunBatchAsync(
                dto.UnitAId,
                dto.UnitBId,
                dto.SimulationCount,
                dto.MaxTurns,
                dto.UnitAPrefersMelee,
                dto.UnitBPrefersMelee,
                cancellationToken);

            return Ok(result);
        }
    }
}
