namespace StatHammer.Server.Models.Entities
{
    public class ModelWeapon
    {
        public int Id { get; set; }

        public int ModelId { get; set; }

        public int WeaponId { get; set; }

        public bool IsDefault { get; set; }

        public Model? Model { get; set; }

        public Weapon? Weapon { get; set; }
    }
}
