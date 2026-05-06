namespace StatHammer.Server.Models.DTOs.Models
{
    public class ModelWeaponReadDto
    {
        public int WeaponId { get; set; }

        public string WeaponName { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
    }
}
