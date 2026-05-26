namespace StatHammer.DesktopClient.Models.Simulations
{
    public class RunSimulationRequestDto
    {
        public int UnitAId { get; set; }

        public int UnitBId { get; set; }

        public int SimulationCount { get; set; } = 1000;

        public int MaxTurns { get; set; } = 5;

        public bool UseParallel { get; set; } = true;

        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

        public bool SaveResult { get; set; } = true;
    }
}