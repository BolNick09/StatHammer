namespace StatHammer.Server.Models.Entities
{
    public class SimulationResult
    {
        public int Id { get; set; }

        public int UnitAId { get; set; }

        public int UnitBId { get; set; }

        public int SimulationCount { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public Unit? UnitA { get; set; }

        public Unit? UnitB { get; set; }

        public ICollection<TurnStat> TurnStats { get; set; } = new List<TurnStat>();
    }
}
