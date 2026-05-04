using StatHammer.Server.Models.DTOs.Units;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Mappings
{
    public static class UnitMappingExtensions
    {
        public static UnitReadDto ToReadDto(this Unit unit)
        {
            return new UnitReadDto
            {
                Id = unit.Id,
                Name = unit.Name,
                Models = unit.UnitModels
                    .Select(um => new UnitModelReadDto
                    {
                        ModelId = um.ModelId,
                        ModelName = um.Model?.Name ?? string.Empty,
                        MinCount = um.MinCount,
                        MaxCount = um.MaxCount
                    })
                    .ToList(),

                Abilities = unit.UnitAbilities
                    .Where(ua => ua.Ability != null)
                    .Select(ua => ua.Ability!.Name)
                    .ToList(),

                Keywords = unit.UnitKeywords
                    .Where(uk => uk.Keyword != null)
                    .Select(uk => uk.Keyword!.Name)
                    .ToList(),

                Options = unit.UnitOptions
                    .Select(uo => new UnitOptionReadDto
                    {
                        Id = uo.Id,
                        Name = uo.Name,
                        MaxSelections = uo.MaxSelections,
                        GroupKey = uo.GroupKey,
                        Items = uo.OptionItems
                            .Select(oi => new OptionItemReadDto
                            {
                                Id = oi.Id,
                                WeaponId = oi.WeaponId,
                                WeaponName = oi.Weapon?.Name,
                                WargearId = oi.WargearId,
                                WargearName = oi.Wargear?.Name
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }

        public static Unit ToEntity(this UnitCreateDto dto)
        {
            return new Unit
            {
                Name = dto.Name
            };
        }
    }
}
