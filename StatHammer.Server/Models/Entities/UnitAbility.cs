namespace StatHammer.Server.Models.Entities
{
    public class UnitAbility
    {
        public int Id { get; set; }

        public int UnitId { get; set; }

        public int AbilityId { get; set; }

        public Unit? Unit { get; set; }

        public Ability? Ability { get; set; }
    }
}
