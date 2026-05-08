namespace StatHammer.Server.Simulation.Combat.Models
{
    public class UnitAttackResult
    {
        public string AttackingUnitName { get; set; } = string.Empty;

        public string DefendingUnitName { get; set; } = string.Empty;

        public int TotalAttacks { get; set; }

        public int TotalHits { get; set; }

        public int TotalWounds { get; set; }

        public int TotalSuccessfulSaves { get; set; }

        public int TotalDamageBeforeFnp { get; set; }

        public int TotalBlockedByFnp { get; set; }

        public int TotalFinalDamage { get; set; }

        public List<AttackResolutionResult> WeaponResults { get; set; } = new();
    }
}
