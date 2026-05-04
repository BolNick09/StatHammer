namespace StatHammer.Server.Models.Entities
{
    public class TurnStat
    {
        public int Id { get; set; }

        public int SimulationResultId { get; set; }

        public int TurnNumber { get; set; }

        public string Side { get; set; } = string.Empty; // "A" или "B"

        public double AvgModelsAlive { get; set; }

        public double AvgWoundsAlive { get; set; }

        public double AvgHits { get; set; }

        public double AvgWounds { get; set; }

        public double AvgSuccessfulSaves { get; set; }

        public double AvgBlockedByFnp { get; set; }

        public SimulationResult? SimulationResult { get; set; }

        public ICollection<WeaponStat> WeaponStats { get; set; } = new List<WeaponStat>();
    }
}
