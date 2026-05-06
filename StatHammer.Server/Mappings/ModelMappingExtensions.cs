using StatHammer.Server.Models.DTOs.Models;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Mappings
{
    public static class ModelMappingExtensions
    {
        public static ModelReadDto ToReadDto(this Models.Entities.Model model)
        {
            return new ModelReadDto
            {
                Id = model.Id,
                Name = model.Name,
                Move = model.Move,
                Toughness = model.Toughness,
                Save = model.Save,
                InvulnerableSave = model.InvulnerableSave,
                Wounds = model.Wounds,
                Leadership = model.Leadership,
                OC = model.OC,
                Weapons = model.ModelWeapons
                    .Where(mw => mw.Weapon != null)
                    .Select(mw => new ModelWeaponReadDto
                    {
                        WeaponId = mw.WeaponId,
                        WeaponName = mw.Weapon!.Name,
                        IsDefault = mw.IsDefault
                    })
                    .ToList(),
                Wargears = model.ModelWargears
                    .Where(mw => mw.Wargear != null)
                    .Select(mw => new ModelWargearReadDto
                    {
                        WargearId = mw.WargearId,
                        WargearName = mw.Wargear!.Name,
                        IsDefault = mw.IsDefault
                    })
                    .ToList(),
                Abilities = model.ModelAbilities
                    .Where(ma => ma.Ability != null)
                    .Select(ma => ma.Ability!.Name)
                    .ToList()
            };
        }

        public static Models.Entities.Model ToEntity(this ModelCreateDto dto)
        {
            return new Models.Entities.Model
            {
                Name = dto.Name,
                Move = dto.Move,
                Toughness = dto.Toughness,
                Save = dto.Save,
                InvulnerableSave = dto.InvulnerableSave,
                Wounds = dto.Wounds,
                Leadership = dto.Leadership,
                OC = dto.OC,
                ModelWeapons = dto.ModelWeapons
                    .Select(w => new ModelWeapon
                    {
                        WeaponId = w.WeaponId,
                        IsDefault = w.IsDefault
                    })
                    .ToList(),
                ModelWargears = dto.ModelWargears
                    .Select(w => new ModelWargear
                    {
                        WargearId = w.WargearId,
                        IsDefault = w.IsDefault
                    })
                    .ToList(),
                ModelAbilities = dto.AbilityIds
                    .Select(id => new ModelAbility
                    {
                        AbilityId = id
                    })
                    .ToList()
            };
        }
    }

}
