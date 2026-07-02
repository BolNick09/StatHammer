using StatHammer.Server.Simulation.Battle.Models;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Battle.Services
{
    public interface IBattleSimulationService
    {
        BattleSimulationResult SimulateBattle(
            SimulationUnit unitA,
            SimulationUnit unitB,
            int maxTurns,
            SimulationModifiers? modifiers = null);
    }
}