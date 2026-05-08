using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Simulation.Combat.DTOs;
using StatHammer.Server.Simulation.Combat.Services;
using StatHammer.Server.Simulation.Models;
using StatHammer.Server.Simulation.Services;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CombatDebugController : ControllerBase
    {
        private readonly IAttackResolver _attackResolver;
        private readonly IUnitAttackResolver _unitAttackResolver;
        private readonly IUnitRuntimeBuilder _unitRuntimeBuilder;

        public CombatDebugController(
            IAttackResolver attackResolver,
            IUnitAttackResolver unitAttackResolver,
            IUnitRuntimeBuilder unitRuntimeBuilder)
        {
            _attackResolver = attackResolver;
            _unitAttackResolver = unitAttackResolver;
            _unitRuntimeBuilder = unitRuntimeBuilder;
        }

        [HttpPost("resolve-attack")]
        public IActionResult ResolveAttack(TestAttackRequestDto dto)
        {
            var attacker = new SimulationModel
            {
                ModelId = 1,
                Name = "Test Attacker",
                Move = 6,
                Toughness = 4,
                Save = 3,
                MaxWounds = 2,
                CurrentWounds = 2,
                Leadership = 7,
                OC = 1
            };

            var defender = new SimulationModel
            {
                ModelId = 2,
                Name = "Test Defender",
                Move = 6,
                Toughness = dto.DefenderToughness,
                Save = dto.DefenderSave,
                InvulnerableSave = dto.DefenderInvulnerableSave,
                FeelNoPain = dto.DefenderFeelNoPain,
                MaxWounds = 3,
                CurrentWounds = 3,
                Leadership = 7,
                OC = 1
            };

            var weapon = new SimulationWeapon
            {
                WeaponId = 1,
                Name = "Test Weapon"
            };

            var profile = new SimulationWeaponProfile
            {
                WeaponProfileId = 1,
                Name = "Test Profile",
                Range = 24,
                Attacks = dto.Attacks,
                Skill = dto.Skill,
                Strength = dto.Strength,
                ArmorPiercing = dto.ArmorPiercing,
                Damage = dto.Damage
            };

            var result = _attackResolver.ResolveAttack(attacker, defender, weapon, profile);

            return Ok(result);
        }

        [HttpPost("resolve-unit-ranged-attack")]
        public async Task<IActionResult> ResolveUnitRangedAttack(UnitAttackTestRequestDto dto)
        {
            var attacker = await _unitRuntimeBuilder.BuildUnitAsync(dto.AttackerUnitId, dto.AttackerPrefersMelee);
            if (attacker == null)
            {
                return BadRequest($"Attacker unit {dto.AttackerUnitId} not found.");
            }

            var defender = await _unitRuntimeBuilder.BuildUnitAsync(dto.DefenderUnitId, dto.DefenderPrefersMelee);
            if (defender == null)
            {
                return BadRequest($"Defender unit {dto.DefenderUnitId} not found.");
            }

            var result = _unitAttackResolver.ResolveRangedAttack(attacker, defender);

            return Ok(result);
        }
    }
}
