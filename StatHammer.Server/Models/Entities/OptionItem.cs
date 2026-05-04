namespace StatHammer.Server.Models.Entities
{
    public class OptionItem
    {
        public int Id { get; set; }

        public int UnitOptionId { get; set; }

        public int? WeaponId { get; set; }

        public int? WargearId { get; set; }

        public UnitOption? UnitOption { get; set; }

        public Weapon? Weapon { get; set; }

        public Wargear? Wargear { get; set; }
    }
}
