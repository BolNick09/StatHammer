namespace StatHammer.Server.Models.Entities
{
    public class UnitModel
    {
        public int Id { get; set; }

        public int UnitId { get; set; }

        public int ModelId { get; set; }

        public int MinCount { get; set; }

        public int MaxCount { get; set; }

        public Unit? Unit { get; set; }

        public Model? Model { get; set; }
    }
}
