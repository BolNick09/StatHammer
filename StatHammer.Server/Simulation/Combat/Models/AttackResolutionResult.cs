namespace StatHammer.Server.Simulation.Combat.Models
{
    public class AttackResolutionResult
    {
        public string WeaponName { get; set; } = string.Empty;

        public string? WeaponProfileName { get; set; }

        public int Attacks { get; set; }

        public int Hits { get; set; }

        public int Wounds { get; set; }

        public int SuccessfulSaves { get; set; }

        public int DamageBeforeFnp { get; set; }

        public int BlockedByFnp { get; set; }

        public int FinalDamage { get; set; }

        public List<int> DamagePackets { get; set; } = new();
    }
}
