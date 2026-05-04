namespace StatHammer.Server.Models.Entities
{
    public class Model
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

        public ICollection<ModelWeapon> ModelWeapons { get; set; } = new List<ModelWeapon>();

        public ICollection<ModelWargear> ModelWargears { get; set; } = new List<ModelWargear>();

        public ICollection<ModelAbility> ModelAbilities { get; set; } = new List<ModelAbility>();
    }
}
