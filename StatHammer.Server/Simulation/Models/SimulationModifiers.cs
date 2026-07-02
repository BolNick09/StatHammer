namespace StatHammer.Server.Simulation.Models
{
    public class SimulationModifiers
    {
        public UnitCombatModifiers UnitA { get; set; } = new();

        public UnitCombatModifiers UnitB { get; set; } = new();

        public static SimulationModifiers None => new();
    }
}