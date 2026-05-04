namespace StatHammer.Server.Models.Entities
{
    public class UnitOption
    {
        public int Id { get; set; }

        public int UnitId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MaxSelections { get; set; }

        public string? GroupKey { get; set; }

        public Unit? Unit { get; set; }

        public ICollection<OptionItem> OptionItems { get; set; } = new List<OptionItem>();
    }
}
