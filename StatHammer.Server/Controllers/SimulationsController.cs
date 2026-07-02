using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Models.DTOs.Simulations;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;
using StatHammer.Server.Simulation.Battle.Services;
using StatHammer.Server.Simulation.Dice.Services;
using StatHammer.Server.Simulation.Models;


namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SimulationsController : ControllerBase
    {
        private readonly IBattleBatchSimulationService _singleThreadService;
        private readonly IBattleBatchSimulationParallelService _parallelService;
        private readonly IBattleResultPersistenceService _persistenceService;

        public SimulationsController(
            IBattleBatchSimulationService singleThreadService,
            IBattleBatchSimulationParallelService parallelService,
            IBattleResultPersistenceService persistenceService)
        {
            _singleThreadService = singleThreadService;
            _parallelService = parallelService;
            _persistenceService = persistenceService;
        }

        [HttpPost("run")]
        public async Task<ActionResult<RunSimulationResponseDto>> Run(
            RunSimulationRequestDto dto,
            CancellationToken cancellationToken)
        {
            if (dto.UnitAId == dto.UnitBId)
            {
                return BadRequest("UnitAId and UnitBId must be different.");
            }
            var modifiers = new SimulationModifiers
            {
                UnitA = new UnitCombatModifiers
                {
                    HitModifier = dto.UnitAHitModifier,
                    WoundModifier = dto.UnitAWoundModifier,
                    ArmorPiercingModifier = dto.UnitAArmorPiercingModifier,
                    SaveModifier = dto.UnitASaveModifier
                },
                UnitB = new UnitCombatModifiers
                {
                    HitModifier = dto.UnitBHitModifier,
                    WoundModifier = dto.UnitBWoundModifier,
                    ArmorPiercingModifier = dto.UnitBArmorPiercingModifier,
                    SaveModifier = dto.UnitBSaveModifier
                }
            };

            BattleSimulationBatchResult result;

            if (dto.UseParallel)
            {
                result = await _parallelService.RunBatchAsync(
                    dto.UnitAId,
                    dto.UnitBId,
                    dto.SimulationCount,
                    dto.MaxTurns,
                    unitAPrefersMelee: false,
                    unitBPrefersMelee: false,
                    dto.MaxDegreeOfParallelism,
                    cancellationToken);
            }
            else
            {
                result = await _singleThreadService.RunBatchAsync(
                    dto.UnitAId,
                    dto.UnitBId,
                    dto.SimulationCount,
                    dto.MaxTurns,
                    unitAPrefersMelee: false,
                    unitBPrefersMelee: false,
                    cancellationToken);
            }

            int? savedId = null;

            if (dto.SaveResult)
            {
                var saved = await _persistenceService.SaveBatchResultAsync(
                    dto.UnitAId,
                    dto.UnitBId,
                    result,
                    cancellationToken);

                savedId = saved.Id;
            }

            return Ok(new RunSimulationResponseDto
            {
                Result = result,
                SavedSimulationResultId = savedId
            });
        }
    }
}