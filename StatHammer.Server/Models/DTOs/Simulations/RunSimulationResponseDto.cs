using StatHammer.Server.Simulation.Battle.Models.Aggregation;

namespace StatHammer.Server.Models.DTOs.Simulations
{
    public class RunSimulationResponseDto
    {
        public BattleSimulationBatchResult Result { get; set; } = new();

        public int? SavedSimulationResultId { get; set; }
    }
}