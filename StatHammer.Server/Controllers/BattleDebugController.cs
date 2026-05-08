using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Simulation.Battle.DTOs;
using StatHammer.Server.Simulation.Battle.Services;
using StatHammer.Server.Simulation.Services;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BattleDebugController : ControllerBase
    {
        private readonly IUnitRuntimeBuilder _unitRuntimeBuilder;
        private readonly IBattleSimulationService _battleSimulationService;

        public BattleDebugController(
            IUnitRuntimeBuilder unitRuntimeBuilder,
            IBattleSimulationService battleSimulationService)
        {
            _unitRuntimeBuilder = unitRuntimeBuilder;
            _battleSimulationService = battleSimulationService;
        }

        [HttpPost("run-single-battle")]
        public async Task<IActionResult> RunSingleBattle(RunSingleBattleRequestDto dto)
        {
            if (dto.MaxTurns < 1 || dto.MaxTurns > 5)
            {
                return BadRequest("MaxTurns must be between 1 and 5.");
            }

            var unitA = await _unitRuntimeBuilder.BuildUnitAsync(dto.UnitAId, dto.UnitAPrefersMelee);
            if (unitA == null)
            {
                return BadRequest($"UnitA {dto.UnitAId} not found.");
            }

            var unitB = await _unitRuntimeBuilder.BuildUnitAsync(dto.UnitBId, dto.UnitBPrefersMelee);
            if (unitB == null)
            {
                return BadRequest($"UnitB {dto.UnitBId} not found.");
            }

            var result = _battleSimulationService.SimulateBattle(unitA, unitB, dto.MaxTurns);

            return Ok(result);
        }
    }
}
