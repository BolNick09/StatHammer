namespace StatHammer.Server.Models.DTOs.Weapons
{
    public class WeaponReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<WeaponProfileReadDto> WeaponProfiles { get; set; } = new();
    }
}
