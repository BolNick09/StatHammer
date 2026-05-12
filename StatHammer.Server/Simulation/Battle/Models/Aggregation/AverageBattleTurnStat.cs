namespace StatHammer.Server.Simulation.Battle.Models.Aggregation
{
    public class AverageBattleTurnStat
    {
        public int TurnNumber { get; set; }

        public AverageSideTurnStat? SideA { get; set; }

        public AverageSideTurnStat? SideB { get; set; }
    }
}
