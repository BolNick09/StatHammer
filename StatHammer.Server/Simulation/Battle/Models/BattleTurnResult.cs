namespace StatHammer.Server.Simulation.Battle.Models
{
    public class BattleTurnResult
    {
        public int TurnNumber { get; set; }

        public BattleSideTurnResult? SideAAction { get; set; }

        public BattleSideTurnResult? SideBAction { get; set; }
    }
}
