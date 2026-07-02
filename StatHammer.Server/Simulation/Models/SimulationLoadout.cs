namespace StatHammer.Server.Simulation.Models
{
    public class SimulationLoadout
    {
        public UnitLoadoutSelection? UnitA { get; set; }

        public UnitLoadoutSelection? UnitB { get; set; }
    }

    public class UnitLoadoutSelection
    {
        public List<UnitModelCountSelection> ModelCounts { get; set; } = new();
    }

    public class UnitModelCountSelection
    {
        public int ModelId { get; set; }

        public int Count { get; set; }
    }
}