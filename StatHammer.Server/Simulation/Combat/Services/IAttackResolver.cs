using StatHammer.Server.Simulation.Combat.Models;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public interface IAttackResolver
    {
        AttackResolutionResult ResolveAttack(
            SimulationModel attacker,
            SimulationModel defender,
            SimulationWeapon weapon,
            SimulationWeaponProfile weaponProfile,
            UnitCombatModifiers? attackerModifiers = null,
            UnitCombatModifiers? defenderModifiers = null);
    }
}