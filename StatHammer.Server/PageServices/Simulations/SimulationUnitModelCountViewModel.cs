using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Simulations
{
    public class SimulationUnitModelCountViewModel
    {
        public int ModelId { get; set; }

        public string ModelName { get; set; } = string.Empty;

        public int MinCount { get; set; }

        public int MaxCount { get; set; }

        [Range(0, 999)]
        public int Count { get; set; }
    }
}