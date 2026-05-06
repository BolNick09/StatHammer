namespace StatHammer.Server.Simulation.DTOs
{
    public class RunSimulationRequestDto
    {
        public int UnitAId { get; set; }

        public int UnitBId { get; set; }

        public int SimulationCount { get; set; }

        public int MaxTurns { get; set; } = 5;

        public int StartingDistance { get; set; } = 24;

        public bool UnitAPrefersMelee { get; set; }

        public bool UnitBPrefersMelee { get; set; }

        public int? RandomSeed { get; set; }
    }

}
