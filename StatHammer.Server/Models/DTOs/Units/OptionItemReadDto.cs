namespace StatHammer.Server.Models.DTOs.Units
{
    public class OptionItemReadDto
    {
        public int Id { get; set; }

        public int? WeaponId { get; set; }

        public string? WeaponName { get; set; }

        public int? WargearId { get; set; }

        public string? WargearName { get; set; }
    }
}
