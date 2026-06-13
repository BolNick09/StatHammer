using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Simulation.Models;
using EntityModel = StatHammer.Server.Models.Entities.Model;

namespace StatHammer.Server.Simulation.Services
{
    public class UnitRuntimeBuilder : IUnitRuntimeBuilder
    {
        private readonly StatHammerDbContext _context;

        public UnitRuntimeBuilder(StatHammerDbContext context)
        {
            _context = context;
        }

        public async Task<SimulationUnit?> BuildUnitAsync(
            int unitId,
            bool preferMelee,
            CancellationToken cancellationToken = default)
        {
            var unit = await _context.Units
            .AsNoTracking()
                .AsSplitQuery()
                    .Include(u => u.UnitModels)
                        .ThenInclude(um => um.Model)
                            .ThenInclude(m => m.ModelWeapons)
                                .ThenInclude(mw => mw.Weapon)
                                    .ThenInclude(w => w.WeaponProfiles)
                                        .ThenInclude(wp => wp.WeaponProfileAbilities)
                                            .ThenInclude(wpa => wpa.Ability)
                .Include(u => u.UnitModels)
                    .ThenInclude(um => um.Model)
                        .ThenInclude(m => m.ModelAbilities)
                            .ThenInclude(ma => ma.Ability)
                .Include(u => u.UnitAbilities)
                    .ThenInclude(ua => ua.Ability)
                .Include(u => u.UnitKeywords)
                    .ThenInclude(uk => uk.Keyword)
                .FirstOrDefaultAsync(u => u.Id == unitId, cancellationToken);

            if (unit == null)
            {
                return null;
            }

            var runtimeUnit = new SimulationUnit
            {
                UnitId = unit.Id,
                Name = unit.Name,
                PreferMelee = preferMelee,
                Abilities = unit.UnitAbilities
                    .Where(ua => ua.Ability != null)
                    .Select(ua => ua.Ability!.Name)
                    .Distinct()
                    .ToList(),
                Keywords = unit.UnitKeywords
                    .Where(uk => uk.Keyword != null)
                    .Select(uk => uk.Keyword!.Name)
                    .Distinct()
                    .ToList()
            };

            foreach (var unitModel in unit.UnitModels)
            {
                if (unitModel.Model == null)                
                    continue;
                

                for (int i = 0; i < unitModel.MinCount; i++)                
                    runtimeUnit.Models.Add(BuildSimulationModel(unitModel.Model));                
            }

            return runtimeUnit;
        }

        private static SimulationModel BuildSimulationModel(EntityModel model)
        {
            return new SimulationModel
            {
                ModelId = model.Id,
                Name = model.Name,
                Move = model.Move,
                Toughness = model.Toughness,
                Save = model.Save,
                InvulnerableSave = model.InvulnerableSave,
                MaxWounds = model.Wounds,
                CurrentWounds = model.Wounds,
                Leadership = model.Leadership,
                OC = model.OC,
                Abilities = model.ModelAbilities
                    .Where(ma => ma.Ability != null)
                    .Select(ma => ma.Ability!.Name)
                    .Distinct()
                    .ToList(),
                Weapons = model.ModelWeapons
                    .Where(mw => mw.Weapon != null)
                    .Select(mw => new SimulationWeapon
                    {
                        WeaponId = mw.WeaponId,
                        Name = mw.Weapon!.Name,
                        Profiles = mw.Weapon.WeaponProfiles
                            .Select(wp => new SimulationWeaponProfile
                            {
                                WeaponProfileId = wp.Id,
                                Name = wp.Name,
                                Range = wp.Range,
                                Attacks = wp.Attacks,
                                Skill = wp.Skill,
                                Strength = wp.Strength,
                                ArmorPiercing = wp.ArmorPiercing,
                                Damage = wp.Damage,
                                Abilities = wp.WeaponProfileAbilities
                                    .Where(wpa => wpa.Ability != null)
                                    .Select(wpa => wpa.Ability!.Name)
                                    .Distinct()
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }

}
