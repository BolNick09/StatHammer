namespace StatHammer.Server.Models.DTOs.Weapons
{
    public class WeaponProfileCreateDto
    {
        public string? Name { get; set; }

        public int Range { get; set; }

        public string Attacks { get; set; } = string.Empty;

        public int Skill { get; set; }

        public int Strength { get; set; }

        public int ArmorPiercing { get; set; }

        public string Damage { get; set; } = string.Empty;
    }
}
