using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Runtime
{
    public static class SimulationRuntimeCloneExtensions
    {
        public static SimulationUnit DeepClone(this SimulationUnit source)
        {
            return new SimulationUnit
            {
                UnitId = source.UnitId,
                Name = source.Name,
                PreferMelee = source.PreferMelee,
                Abilities = source.Abilities.ToList(),
                Keywords = source.Keywords.ToList(),
                Models = source.Models
                    .Select(model => model.DeepClone())
                    .ToList()
            };
        }

        public static SimulationModel DeepClone(this SimulationModel source)
        {
            return new SimulationModel
            {
                ModelId = source.ModelId,
                Name = source.Name,
                Move = source.Move,
                Toughness = source.Toughness,
                Save = source.Save,
                InvulnerableSave = source.InvulnerableSave,
                MaxWounds = source.MaxWounds,
                CurrentWounds = source.CurrentWounds,
                Leadership = source.Leadership,
                OC = source.OC,
                FeelNoPain = source.FeelNoPain,
                Abilities = source.Abilities.ToList(),
                Weapons = source.Weapons
                    .Select(weapon => weapon.DeepClone())
                    .ToList()
            };
        }

        public static SimulationWeapon DeepClone(this SimulationWeapon source)
        {
            return new SimulationWeapon
            {
                WeaponId = source.WeaponId,
                Name = source.Name,
                Profiles = source.Profiles
                    .Select(profile => profile.DeepClone())
                    .ToList()
            };
        }

        public static SimulationWeaponProfile DeepClone(this SimulationWeaponProfile source)
        {
            return new SimulationWeaponProfile
            {
                WeaponProfileId = source.WeaponProfileId,
                Name = source.Name,
                Range = source.Range,
                Attacks = source.Attacks,
                Skill = source.Skill,
                Strength = source.Strength,
                ArmorPiercing = source.ArmorPiercing,
                Damage = source.Damage,
                Abilities = source.Abilities.ToList()
            };
        }
    }
}