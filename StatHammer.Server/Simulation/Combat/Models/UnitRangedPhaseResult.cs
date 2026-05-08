namespace StatHammer.Server.Simulation.Combat.Models
{
    public class UnitRangedPhaseResult
    {
        public UnitAttackResult AttackResult { get; set; } = new();

        public DamageAllocationResult AllocationResult { get; set; } = new();
    }
}
