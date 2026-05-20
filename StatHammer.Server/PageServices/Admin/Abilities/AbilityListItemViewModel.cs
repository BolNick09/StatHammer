namespace StatHammer.Server.PageServices.Admin.Abilities
{
    public class AbilityListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int WeaponProfileUsageCount { get; set; }

        public int ModelUsageCount { get; set; }

        public int UnitUsageCount { get; set; }

        public int TotalUsageCount =>
            WeaponProfileUsageCount + ModelUsageCount + UnitUsageCount;

        public bool CanDelete => TotalUsageCount == 0;
    }
}
