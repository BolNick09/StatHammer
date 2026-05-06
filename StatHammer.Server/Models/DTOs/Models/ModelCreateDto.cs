namespace StatHammer.Server.Models.DTOs.Models
{
    public class ModelCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public int Move { get; set; }

        public int Toughness { get; set; }

        public int Save { get; set; }

        public int? InvulnerableSave { get; set; }

        public int Wounds { get; set; }

        public int Leadership { get; set; }

        public int OC { get; set; }

        public List<ModelWeaponCreateDto> ModelWeapons { get; set; } = new();

        public List<ModelWargearCreateDto> ModelWargears { get; set; } = new();

        public List<int> AbilityIds { get; set; } = new();
    }

}
