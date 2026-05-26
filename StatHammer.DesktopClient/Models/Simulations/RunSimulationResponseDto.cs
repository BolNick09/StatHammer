namespace StatHammer.DesktopClient.Models.Simulations
{
    public class RunSimulationResponseDto
    {
        public BattleSimulationBatchResultDto Result { get; set; } = new();

        public int? SavedSimulationResultId { get; set; }
    }
}