using StatHammer.Server.Simulation.Combat.Models;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public class DamageAllocator : IDamageAllocator
    {
        public DamageAllocationResult ApplyDamage(SimulationUnit defender, int incomingDamage)
        {
            var result = new DamageAllocationResult
            {
                DefendingUnitName = defender.Name,
                IncomingDamage = incomingDamage
            };

            if (incomingDamage <= 0)
            {
                result.RemainingAliveModels = defender.AliveModelCount;
                result.RemainingTotalWounds = defender.TotalCurrentWounds;
                return result;
            }

            var aliveModelsBefore = defender.AliveModelCount;
            var damageLeft = incomingDamage;

            foreach (var model in defender.Models.Where(m => m.IsAlive))
            {
                if (damageLeft <= 0)
                {
                    break;
                }

                var damageToApply = Math.Min(model.CurrentWounds, damageLeft);

                model.CurrentWounds -= damageToApply;
                damageLeft -= damageToApply;
                result.AppliedDamage += damageToApply;
            }

            result.WastedDamage = damageLeft;
            result.ModelsKilled = aliveModelsBefore - defender.AliveModelCount;
            result.RemainingAliveModels = defender.AliveModelCount;
            result.RemainingTotalWounds = defender.TotalCurrentWounds;

            return result;
        }
    }
}
