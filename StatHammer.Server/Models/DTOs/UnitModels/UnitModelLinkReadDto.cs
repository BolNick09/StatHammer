namespace StatHammer.Server.Models.DTOs.UnitModels
{
    public class UnitModelLinkReadDto
    {
        public int Id { get; set; }

        public int UnitId { get; set; }

        public string UnitName { get; set; } = string.Empty;

        public int ModelId { get; set; }

        public string ModelName { get; set; } = string.Empty;

        public int MinCount { get; set; }

        public int MaxCount { get; set; }
    }

}
