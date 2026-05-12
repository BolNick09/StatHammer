namespace StatHammer.Server.Simulation.Battle.DTOs
{
    public class RunBattleParallelBatchRequestDto
    {
        public int UnitAId { get; set; }

        public int UnitBId { get; set; }

        public int SimulationCount { get; set; }

        public int MaxTurns { get; set; } = 5;

        public bool UnitAPrefersMelee { get; set; }

        public bool UnitBPrefersMelee { get; set; }

        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    }
}
