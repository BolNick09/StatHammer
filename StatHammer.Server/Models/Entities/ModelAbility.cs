namespace StatHammer.Server.Models.Entities
{
    public class ModelAbility
    {
        public int Id { get; set; }

        public int ModelId { get; set; }

        public int AbilityId { get; set; }

        public Model? Model { get; set; }

        public Ability? Ability { get; set; }
    }
}
