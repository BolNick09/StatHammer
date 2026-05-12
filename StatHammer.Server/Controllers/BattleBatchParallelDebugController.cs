using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Simulation.Battle.DTOs;
using StatHammer.Server.Simulation.Battle.Services;
using StatHammer.Server.Simulation.Dice.Services;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BattleBatchParallelDebugController : ControllerBase
    {
        private readonly IBattleBatchSimulationParallelService _parallelService;
        private readonly IBattleResultPersistenceService _battleResultPersistenceService;

        public BattleBatchParallelDebugController(
            IBattleBatchSimulationParallelService parallelService,
            IBattleResultPersistenceService battleResultPersistenceService)
        {
            _parallelService = parallelService;
            _battleResultPersistenceService = battleResultPersistenceService;
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

        [HttpPost("run-batch-and-save")]
        public async Task<IActionResult> RunBatchAndSave(
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

            var batchResult = await _parallelService.RunBatchAsync(
                dto.UnitAId,
                dto.UnitBId,
                dto.SimulationCount,
                dto.MaxTurns,
                dto.UnitAPrefersMelee,
                dto.UnitBPrefersMelee,
                dto.MaxDegreeOfParallelism,
                cancellationToken);

            var savedResult = await _battleResultPersistenceService.SaveBatchResultAsync(
                dto.UnitAId,
                dto.UnitBId,
                batchResult,
                cancellationToken);

            return Ok(new
            {
                batchResult,
                savedSimulationResultId = savedResult.Id
            });
        }
    }
}
