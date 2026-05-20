namespace StatHammer.Server.PageServices.Admin.Units
{
    public class UnitModelCompositionViewModel
    {
        public int ModelId { get; set; }

        public string ModelName { get; set; } = string.Empty;

        public int MinCount { get; set; }

        public int MaxCount { get; set; }
    }
}