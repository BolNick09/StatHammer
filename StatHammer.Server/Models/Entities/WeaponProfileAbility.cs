namespace StatHammer.Server.Models.Entities
{
    public class WeaponProfileAbility
    {
        public int Id { get; set; }

        public int WeaponProfileId { get; set; }

        public int AbilityId { get; set; }

        public WeaponProfile? WeaponProfile { get; set; }

        public Ability? Ability { get; set; }
    }
}
