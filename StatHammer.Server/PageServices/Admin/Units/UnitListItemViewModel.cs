namespace StatHammer.Server.PageServices.Admin.Units
{
    public class UnitListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<UnitModelCompositionViewModel> Models { get; set; } = new();

        public List<string> Keywords { get; set; } = new();

        public int SimulationResultUsageCount { get; set; }

        public bool CanDelete => SimulationResultUsageCount == 0;
    }
}