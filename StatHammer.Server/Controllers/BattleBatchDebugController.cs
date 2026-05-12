using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Simulation.Battle.DTOs;
using StatHammer.Server.Simulation.Battle.Services;
using StatHammer.Server.Simulation.Dice.Services;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BattleBatchDebugController : ControllerBase
    {
        private readonly IBattleBatchSimulationService _battleBatchSimulationService;
        private readonly IBattleResultPersistenceService _battleResultPersistenceService;

        public BattleBatchDebugController(
            IBattleBatchSimulationService battleBatchSimulationService,
            IBattleResultPersistenceService battleResultPersistenceService)
        {
            _battleBatchSimulationService = battleBatchSimulationService;
            _battleResultPersistenceService = battleResultPersistenceService;
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
        [HttpPost("run-batch-and-save")]
        public async Task<IActionResult> RunBatchAndSave(
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

            var batchResult = await _battleBatchSimulationService.RunBatchAsync(
                dto.UnitAId,
                dto.UnitBId,
                dto.SimulationCount,
                dto.MaxTurns,
                dto.UnitAPrefersMelee,
                dto.UnitBPrefersMelee,
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
