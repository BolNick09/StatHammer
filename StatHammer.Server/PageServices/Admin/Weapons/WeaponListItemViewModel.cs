namespace StatHammer.Server.PageServices.Admin.Weapons
{
    public class WeaponListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<WeaponProfileListItemViewModel> Profiles { get; set; } = new();

        public List<string> UsedByModels { get; set; } = new();

        public int SimulationResultUsageCount { get; set; }

        public bool IsUsedByModels => UsedByModels.Any();

        public bool IsUsedBySimulationResults => SimulationResultUsageCount > 0;

        public bool CanDelete => !IsUsedByModels && !IsUsedBySimulationResults;
    }
}