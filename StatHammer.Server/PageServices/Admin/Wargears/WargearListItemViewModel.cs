namespace StatHammer.Server.PageServices.Admin.Wargears
{
    public class WargearListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<string> UsedByModels { get; set; } = new();

        public bool CanDelete => !UsedByModels.Any();
    }
}