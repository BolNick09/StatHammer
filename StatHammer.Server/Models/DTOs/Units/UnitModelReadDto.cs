namespace StatHammer.Server.Models.DTOs.Units
{
    public class UnitModelReadDto
    {
        public int ModelId { get; set; }

        public string ModelName { get; set; } = string.Empty;

        public int MinCount { get; set; }

        public int MaxCount { get; set; }
    }
}
