namespace StatHammer.Server.Models.DTOs.Weapons
{
    public class WeaponCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public List<WeaponProfileCreateDto> WeaponProfiles { get; set; } = new();
    }

}
