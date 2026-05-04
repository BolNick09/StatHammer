namespace StatHammer.Server.Models.Entities
{
    public class UnitKeyword
    {
        public int Id { get; set; }

        public int UnitId { get; set; }

        public int KeywordId { get; set; }

        public Unit? Unit { get; set; }

        public Keyword? Keyword { get; set; }
    }
}
