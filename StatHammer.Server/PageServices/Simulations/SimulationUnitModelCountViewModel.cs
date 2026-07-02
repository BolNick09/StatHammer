using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Simulations
{
    public class SimulationUnitModelCountViewModel
    {
        public int ModelId { get; set; }

        public string ModelName { get; set; } = string.Empty;

        public int MinCount { get; set; }

        public int MaxCount { get; set; }

        [Range(0, 999)]
        public int Count { get; set; }

        public int Move { get; set; }

        public int Toughness { get; set; }

        public int Save { get; set; }

        public int? InvulnerableSave { get; set; }

        public int Wounds { get; set; }

        public int Leadership { get; set; }

        public int OC { get; set; }

        public string SaveDisplay =>
            InvulnerableSave.HasValue
                ? $"{Save}+ / {InvulnerableSave.Value}++"
                : $"{Save}+";

        public string LeadershipDisplay => $"{Leadership}+";

        public List<SimulationUnitWeaponProfileViewModel> WeaponProfiles { get; set; } = new();
    }

    public class SimulationUnitWeaponProfileViewModel
    {
        public string WeaponName { get; set; } = string.Empty;

        public string? ProfileName { get; set; }

        public int Range { get; set; }

        public string RangeDisplay => Range == 0 ? "Melee" : $"{Range}\"";

        public string Attacks { get; set; } = string.Empty;

        public int Skill { get; set; }

        public string SkillDisplay => $"{Skill}+";

        public int Strength { get; set; }

        public int ArmorPiercing { get; set; }

        public string Damage { get; set; } = string.Empty;

        public List<string> Abilities { get; set; } = new();

        public string AbilityDisplay => Abilities.Any()
            ? string.Join(", ", Abilities)
            : "—";
    }
}