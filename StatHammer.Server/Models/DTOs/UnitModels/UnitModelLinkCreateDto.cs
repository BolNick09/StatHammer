namespace StatHammer.Server.Models.DTOs.UnitModels
{
    public class UnitModelLinkCreateDto
    {
        public int UnitId { get; set; }

        public int ModelId { get; set; }

        public int MinCount { get; set; }

        public int MaxCount { get; set; }
    }
}
