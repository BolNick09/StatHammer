namespace StatHammer.Server.Simulation.Battle.Models
{
    public class BattleSideTurnResult
    {
        public string Side { get; set; } = string.Empty;

        public string AttackingUnitName { get; set; } = string.Empty;

        public string TargetUnitName { get; set; } = string.Empty;

        public int StartingTargetAliveModels { get; set; }

        public int StartingTargetTotalWounds { get; set; }

        public int EndingTargetAliveModels { get; set; }

        public int EndingTargetTotalWounds { get; set; }

        public int TotalAttacks { get; set; }

        public int TotalHits { get; set; }

        public int TotalWounds { get; set; }

        public int TotalSuccessfulSaves { get; set; }

        public int TotalDamageBeforeFnp { get; set; }

        public int TotalBlockedByFnp { get; set; }

        public int TotalFinalDamage { get; set; }

        public int ModelsKilled { get; set; }

        public List<Simulation.Combat.Models.GroupedWeaponAttackResult> WeaponResults { get; set; } = new();
    }
}
