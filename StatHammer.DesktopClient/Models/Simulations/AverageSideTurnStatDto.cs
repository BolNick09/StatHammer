namespace StatHammer.DesktopClient.Models.Simulations
{
    public class AverageSideTurnStatDto
    {
        public string Side { get; set; } = string.Empty;

        public double AverageStartingTargetAliveModels { get; set; }

        public double AverageStartingTargetTotalWounds { get; set; }

        public double AverageEndingTargetAliveModels { get; set; }

        public double AverageEndingTargetTotalWounds { get; set; }

        public double AverageAttacks { get; set; }

        public double AverageHits { get; set; }

        public double AverageWounds { get; set; }

        public double AverageSuccessfulSaves { get; set; }

        public double AverageDamageBeforeFnp { get; set; }

        public double AverageBlockedByFnp { get; set; }

        public double AverageFinalDamage { get; set; }

        public double AverageModelsKilled { get; set; }

        public List<AverageWeaponTurnStatDto> WeaponStats { get; set; } = new();
    }
}