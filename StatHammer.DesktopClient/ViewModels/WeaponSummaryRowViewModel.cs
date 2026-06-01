namespace StatHammer.DesktopClient.ViewModels
{
    public class WeaponSummaryRowViewModel
    {
        public int TurnNumber { get; set; }

        public string Side { get; set; } = string.Empty;

        public string WeaponName { get; set; } = string.Empty;

        public string ProfileName { get; set; } = string.Empty;

        public double AverageCount { get; set; }

        public double AverageAttacks { get; set; }

        public double AverageHits { get; set; }

        public double AverageWounds { get; set; }

        public double AverageFinalDamage { get; set; }
    }
}