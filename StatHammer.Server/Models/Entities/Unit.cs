using Microsoft.VisualBasic.FileIO;

namespace StatHammer.Server.Models.Entities
{
    public class Unit
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<UnitModel> UnitModels { get; set; } = new List<UnitModel>();

        public ICollection<UnitAbility> UnitAbilities { get; set; } = new List<UnitAbility>();

        public ICollection<UnitKeyword> UnitKeywords { get; set; } = new List<UnitKeyword>();

        public ICollection<UnitOption> UnitOptions { get; set; } = new List<UnitOption>();
    }
}
