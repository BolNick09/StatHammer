namespace StatHammer.Server.Simulation.Combat.Models
{
    public class DamageAllocationResult
    {
        public string DefendingUnitName { get; set; } = string.Empty;

        public int IncomingDamage { get; set; }

        public int AppliedDamage { get; set; }

        public int WastedDamage { get; set; }

        public int ModelsKilled { get; set; }

        public int RemainingAliveModels { get; set; }

        public int RemainingTotalWounds { get; set; }
    }

}
