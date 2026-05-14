using StatHammer.Server.Simulation.Battle.Models.Aggregation;

namespace StatHammer.Server.PageServices.Simulations
{
    public class SimulationRunViewModel
    {
        public BattleSimulationBatchResult? Result { get; set; }

        public int? SavedSimulationResultId { get; set; }

        public bool WasSaved => SavedSimulationResultId.HasValue;
    }

}
