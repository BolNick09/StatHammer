namespace StatHammer.DesktopClient.Models.Simulations
{
    public class AverageWeaponTurnStatDto
    {
        public string WeaponName { get; set; } = string.Empty;

        public string? WeaponProfileName { get; set; }

        public double AverageCount { get; set; }

        public double AverageAttacks { get; set; }

        public double AverageHits { get; set; }

        public double AverageWounds { get; set; }

        public double AverageSuccessfulSaves { get; set; }

        public double AverageDamageBeforeFnp { get; set; }

        public double AverageBlockedByFnp { get; set; }

        public double AverageFinalDamage { get; set; }
    }
}