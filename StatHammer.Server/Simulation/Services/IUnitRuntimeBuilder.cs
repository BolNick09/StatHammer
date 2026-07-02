using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Services
{
    public interface IUnitRuntimeBuilder
    {
        Task<SimulationUnit?> BuildUnitAsync(
            int unitId,
            bool preferMelee,
            CancellationToken cancellationToken = default,
            UnitLoadoutSelection? loadout = null);
    }
}