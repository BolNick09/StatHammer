using StatHammer.Server.Simulation.Combat.Models;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public class DamageAllocator : IDamageAllocator
    {
        public DamageAllocationResult ApplyDamage(SimulationUnit defender, IReadOnlyCollection<int> damagePackets)
        {
            var result = new DamageAllocationResult
            {
                DefendingUnitName = defender.Name,
                IncomingDamage = damagePackets.Sum()
            };

            if (damagePackets.Count == 0)
            {
                result.RemainingAliveModels = defender.AliveModelCount;
                result.RemainingTotalWounds = defender.TotalCurrentWounds;
                return result;
            }

            var aliveModelsBefore = defender.AliveModelCount;

            foreach (var packet in damagePackets)
            {
                if (packet <= 0)
                {
                    continue;
                }

                var targetModel = defender.Models.FirstOrDefault(m => m.IsAlive);
                if (targetModel == null)
                {
                    result.WastedDamage += packet;
                    continue;
                }

                var appliedToThisModel = Math.Min(targetModel.CurrentWounds, packet);
                targetModel.CurrentWounds -= appliedToThisModel;
                result.AppliedDamage += appliedToThisModel;

                var wastedFromPacket = packet - appliedToThisModel;
                result.WastedDamage += wastedFromPacket;
            }

            result.ModelsKilled = aliveModelsBefore - defender.AliveModelCount;
            result.RemainingAliveModels = defender.AliveModelCount;
            result.RemainingTotalWounds = defender.TotalCurrentWounds;

            return result;
        }
    }
}
