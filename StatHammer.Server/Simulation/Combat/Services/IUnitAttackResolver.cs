using StatHammer.Server.Simulation.Combat.Models;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public interface IUnitAttackResolver
    {
        UnitAttackResult ResolveRangedAttack(
            SimulationUnit attacker,
            SimulationUnit defender);
    }
}
