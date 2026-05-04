namespace StatHammer.Server.Models.Entities
{
    public class Weapon
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<WeaponProfile> WeaponProfiles { get; set; } = new List<WeaponProfile>();

        public ICollection<ModelWeapon> ModelWeapons { get; set; } = new List<ModelWeapon>();
    }
}
