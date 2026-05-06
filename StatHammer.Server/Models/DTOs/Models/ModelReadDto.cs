namespace StatHammer.Server.Models.DTOs.Models
{
    public class ModelReadDto
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

        public List<ModelWeaponReadDto> Weapons { get; set; } = new();

        public List<ModelWargearReadDto> Wargears { get; set; } = new();

        public List<string> Abilities { get; set; } = new();
    }

}
