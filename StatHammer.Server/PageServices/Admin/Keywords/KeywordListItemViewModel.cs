namespace StatHammer.Server.PageServices.Admin.Keywords
{
    public class KeywordListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<string> UsedByUnits { get; set; } = new();

        public bool CanDelete => !UsedByUnits.Any();
    }
}