namespace StatHammer.Server.Models.DTOs.Units
{
    public class UnitReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<UnitModelReadDto> Models { get; set; } = new();

        public List<string> Abilities { get; set; } = new();

        public List<string> Keywords { get; set; } = new();

        public List<UnitOptionReadDto> Options { get; set; } = new();
    }
}
