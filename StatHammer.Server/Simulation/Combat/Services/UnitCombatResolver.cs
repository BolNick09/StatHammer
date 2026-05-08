using StatHammer.Server.Simulation.Combat.Models;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public class UnitCombatResolver : IUnitCombatResolver
    {
        private readonly IUnitAttackResolver _unitAttackResolver;
        private readonly IDamageAllocator _damageAllocator;

        public UnitCombatResolver(
            IUnitAttackResolver unitAttackResolver,
            IDamageAllocator damageAllocator)
        {
            _unitAttackResolver = unitAttackResolver;
            _damageAllocator = damageAllocator;
        }

        public UnitRangedPhaseResult ResolveRangedPhase(
            SimulationUnit attacker,
            SimulationUnit defender)
        {
            var attackResult = _unitAttackResolver.ResolveRangedAttack(attacker, defender);

            var allDamagePackets = attackResult.WeaponResults
                .SelectMany(wr => wr.DamagePackets)
                .ToList();

            var allocationResult = _damageAllocator.ApplyDamage(defender, allDamagePackets);

            return new UnitRangedPhaseResult
            {
                AttackResult = attackResult,
                AllocationResult = allocationResult
            };
        }
    }
}
