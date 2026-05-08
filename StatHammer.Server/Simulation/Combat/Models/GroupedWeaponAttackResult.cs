namespace StatHammer.Server.Simulation.Combat.Models
{
    public class GroupedWeaponAttackResult
    {
        public string WeaponName { get; set; } = string.Empty;

        public string? WeaponProfileName { get; set; }

        public int Count { get; set; }

        public int TotalAttacks { get; set; }

        public int TotalHits { get; set; }

        public int TotalWounds { get; set; }

        public int TotalSuccessfulSaves { get; set; }

        public int TotalDamageBeforeFnp { get; set; }

        public int TotalBlockedByFnp { get; set; }

        public int TotalFinalDamage { get; set; }
    }
}
