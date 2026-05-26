using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.Models.DTOs.Simulations
{
    public class RunSimulationRequestDto
    {
        [Required]
        public int UnitAId { get; set; }

        [Required]
        public int UnitBId { get; set; }

        [Range(1, 100000)]
        public int SimulationCount { get; set; } = 1000;

        [Range(1, 5)]
        public int MaxTurns { get; set; } = 5;

        public bool UseParallel { get; set; } = true;

        [Range(1, 64)]
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

        public bool SaveResult { get; set; } = true;
    }
}