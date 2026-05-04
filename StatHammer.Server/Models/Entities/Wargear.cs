namespace StatHammer.Server.Models.Entities
{
    public class Wargear
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<ModelWargear> ModelWargears { get; set; } = new List<ModelWargear>();
    }
}
