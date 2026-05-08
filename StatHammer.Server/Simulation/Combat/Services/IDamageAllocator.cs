using StatHammer.Server.Simulation.Combat.Models;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public interface IDamageAllocator
    {
        DamageAllocationResult ApplyDamage(SimulationUnit defender, IReadOnlyCollection<int> damagePackets);
    }
}
