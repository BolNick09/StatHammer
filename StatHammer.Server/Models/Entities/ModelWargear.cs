namespace StatHammer.Server.Models.Entities
{
    public class ModelWargear
    {
        public int Id { get; set; }

        public int ModelId { get; set; }

        public int WargearId { get; set; }

        public bool IsDefault { get; set; }

        public Model? Model { get; set; }

        public Wargear? Wargear { get; set; }
    }
}
