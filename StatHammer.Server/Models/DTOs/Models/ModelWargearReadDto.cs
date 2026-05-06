namespace StatHammer.Server.Models.DTOs.Models
{
    public class ModelWargearReadDto
    {
        public int WargearId { get; set; }

        public string WargearName { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
    }
}
