namespace StatHammer.DesktopClient.ViewModels
{
    public class TurnSummaryRowViewModel
    {
        public int TurnNumber { get; set; }

        public string Side { get; set; } = string.Empty;

        public double AverageAttacks { get; set; }

        public double AverageHits { get; set; }

        public double AverageWounds { get; set; }

        public double AverageSuccessfulSaves { get; set; }

        public double AverageFinalDamage { get; set; }

        public double AverageModelsKilled { get; set; }
    }
}