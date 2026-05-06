using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Simulation.Combat.DTOs;
using StatHammer.Server.Simulation.Combat.Services;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CombatDebugController : ControllerBase
    {
        private readonly IAttackResolver _attackResolver;

        public CombatDebugController(IAttackResolver attackResolver)
        {
            _attackResolver = attackResolver;
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
    }
}
