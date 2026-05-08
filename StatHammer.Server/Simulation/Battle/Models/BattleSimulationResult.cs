namespace StatHammer.Server.Simulation.Battle.Models
{
    public class BattleSimulationResult
    {
        public string UnitAName { get; set; } = string.Empty;

        public string UnitBName { get; set; } = string.Empty;

        public int MaxTurns { get; set; }

        public int CompletedTurns { get; set; }

        public bool UnitADestroyed { get; set; }

        public bool UnitBDestroyed { get; set; }

        public string Outcome { get; set; } = string.Empty;
        // "UnitA", "UnitB", "Draw"

        public int UnitAFinalAliveModels { get; set; }

        public int UnitAFinalTotalWounds { get; set; }

        public int UnitBFinalAliveModels { get; set; }

        public int UnitBFinalTotalWounds { get; set; }

        public List<BattleTurnResult> Turns { get; set; } = new();
    }
}
