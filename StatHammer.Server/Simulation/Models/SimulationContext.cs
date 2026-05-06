namespace StatHammer.Server.Simulation.Models
{
    public class SimulationContext
    {
        public int MaxTurns { get; set; } = 5;

        public int StartingDistance { get; set; } = 24;

        public bool UnitAPrefersMelee { get; set; }

        public bool UnitBPrefersMelee { get; set; }

        public int RandomSeed { get; set; }
    }
}
