using StatHammer.Server.Models.DTOs.Weapons;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Mappings
{
    public static class WeaponMappingExtensions
    {
        public static WeaponReadDto ToReadDto(this Weapon weapon)
        {
            return new WeaponReadDto
            {
                Id = weapon.Id,
                Name = weapon.Name,
                WeaponProfiles = weapon.WeaponProfiles
                    .Select(wp => new WeaponProfileReadDto
                    {
                        Id = wp.Id,
                        Name = wp.Name,
                        Range = wp.Range,
                        Attacks = wp.Attacks,
                        Skill = wp.Skill,
                        Strength = wp.Strength,
                        ArmorPiercing = wp.ArmorPiercing,
                        Damage = wp.Damage,
                        Abilities = wp.WeaponProfileAbilities
                            .Where(x => x.Ability != null)
                            .Select(x => x.Ability!.Name)
                            .ToList()
                    })
                    .ToList()
            };
        }

        public static Weapon ToEntity(this WeaponCreateDto dto)
        {
            return new Weapon
            {
                Name = dto.Name,
                WeaponProfiles = dto.WeaponProfiles
                    .Select(wp => new WeaponProfile
                    {
                        Name = wp.Name,
                        Range = wp.Range,
                        Attacks = wp.Attacks,
                        Skill = wp.Skill,
                        Strength = wp.Strength,
                        ArmorPiercing = wp.ArmorPiercing,
                        Damage = wp.Damage,
                        WeaponProfileAbilities = new List<WeaponProfileAbility>()
                    })
                    .ToList()
            };
        }
    }

}
