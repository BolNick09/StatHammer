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

        [Range(-3, 3)]
        public int UnitAHitModifier { get; set; }

        [Range(-3, 3)]
        public int UnitAWoundModifier { get; set; }

        [Range(-3, 3)]
        public int UnitAArmorPiercingModifier { get; set; }

        [Range(-3, 3)]
        public int UnitASaveModifier { get; set; }

        [Range(-3, 3)]
        public int UnitBHitModifier { get; set; }

        [Range(-3, 3)]
        public int UnitBWoundModifier { get; set; }

        [Range(-3, 3)]
        public int UnitBArmorPiercingModifier { get; set; }

        [Range(-3, 3)]
        public int UnitBSaveModifier { get; set; }
    }
}