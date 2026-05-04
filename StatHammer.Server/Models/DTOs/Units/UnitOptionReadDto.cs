namespace StatHammer.Server.Models.DTOs.Units
{
    public class UnitOptionReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MaxSelections { get; set; }

        public string? GroupKey { get; set; }

        public List<OptionItemReadDto> Items { get; set; } = new();
    }
}
