namespace StatHammer.Server.Models.DTOs.Simulations
{
    public class SimulationResultReadDto
    {
        public int Id { get; set; }

        public int UnitAId { get; set; }

        public string UnitAName { get; set; } = string.Empty;

        public int UnitBId { get; set; }

        public string UnitBName { get; set; } = string.Empty;

        public int SimulationCount { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public List<TurnStatReadDto> TurnStats { get; set; } = new();
    }
}
