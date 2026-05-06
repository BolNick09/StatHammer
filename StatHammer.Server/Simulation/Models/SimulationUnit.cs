namespace StatHammer.Server.Simulation.Models
{
    public class SimulationUnit
    {
        public int UnitId { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<SimulationModel> Models { get; set; } = new();

        public List<string> Abilities { get; set; } = new();

        public List<string> Keywords { get; set; } = new();

        public bool PreferMelee { get; set; }

        public bool IsDestroyed => Models.Count == 0 || Models.All(m => m.CurrentWounds <= 0);

        public int AliveModelCount => Models.Count(m => m.CurrentWounds > 0);

        public int TotalCurrentWounds => Models
            .Where(m => m.CurrentWounds > 0)
            .Sum(m => m.CurrentWounds);
    }

}
