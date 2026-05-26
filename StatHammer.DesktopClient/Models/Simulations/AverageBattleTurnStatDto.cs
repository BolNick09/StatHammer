namespace StatHammer.DesktopClient.Models.Simulations
{
    public class AverageBattleTurnStatDto
    {
        public int TurnNumber { get; set; }

        public AverageSideTurnStatDto? SideA { get; set; }

        public AverageSideTurnStatDto? SideB { get; set; }
    }
}