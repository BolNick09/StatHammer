namespace StatHammer.Server.Models.DTOs.Simulations
{
    public class SimulationResultCreateDto
    {
        public int UnitAId { get; set; }

        public int UnitBId { get; set; }

        public int SimulationCount { get; set; }

        public List<TurnStatCreateDto> TurnStats { get; set; } = new();
    }
}
