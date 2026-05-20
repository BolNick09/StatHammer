namespace StatHammer.Server.PageServices.Admin.Models
{
    public class ModelListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Move { get; set; }

        public int Toughness { get; set; }

        public int Save { get; set; }

        public int? InvulnerableSave { get; set; }

        public int Wounds { get; set; }

        public int Leadership { get; set; }

        public int OC { get; set; }

        public List<ModelEquipmentItemViewModel> Weapons { get; set; } = new();

        public List<ModelEquipmentItemViewModel> Wargears { get; set; } = new();

        public List<ModelEquipmentItemViewModel> Abilities { get; set; } = new();

        public List<string> UsedByUnits { get; set; } = new();

        public bool IsUsedByUnits => UsedByUnits.Any();

        public bool CanDelete => !IsUsedByUnits;
    }
}