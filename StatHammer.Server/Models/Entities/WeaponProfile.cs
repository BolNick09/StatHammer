namespace StatHammer.Server.Models.Entities
{
    public class WeaponProfile
    {
        public int Id { get; set; }

        public int WeaponId { get; set; }

        public string? Name { get; set; }

        public int Range { get; set; }

        public string Attacks { get; set; } = string.Empty;

        public int Skill { get; set; }

        public int Strength { get; set; }

        public int ArmorPiercing { get; set; }

        public string Damage { get; set; } = string.Empty;

        public Weapon? Weapon { get; set; }

        public ICollection<WeaponProfileAbility> WeaponProfileAbilities { get; set; } = new List<WeaponProfileAbility>();
    }
}
