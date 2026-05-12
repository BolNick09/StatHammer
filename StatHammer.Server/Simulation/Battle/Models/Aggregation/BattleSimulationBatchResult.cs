namespace StatHammer.Server.Simulation.Battle.Models.Aggregation
{
    public class BattleSimulationBatchResult
    {
        public string UnitAName { get; set; } = string.Empty;

        public string UnitBName { get; set; } = string.Empty;

        public int SimulationCount { get; set; }

        public int MaxTurns { get; set; }

        public int UnitAWins { get; set; }

        public int UnitBWins { get; set; }

        public int Draws { get; set; }

        public double AverageCompletedTurns { get; set; }

        public double AverageUnitAFinalAliveModels { get; set; }

        public double AverageUnitAFinalTotalWounds { get; set; }

        public double AverageUnitBFinalAliveModels { get; set; }

        public double AverageUnitBFinalTotalWounds { get; set; }

        public List<AverageBattleTurnStat> Turns { get; set; } = new();
    }
}
