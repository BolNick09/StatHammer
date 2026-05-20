using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.PageServices.Admin.Weapons
{
    public class WeaponAdminPageService : IWeaponAdminPageService
    {
        private readonly StatHammerDbContext _context;

        public WeaponAdminPageService(StatHammerDbContext context)
        {
            _context = context;
        }

        public async Task<List<WeaponListItemViewModel>> GetWeaponsAsync(
            CancellationToken cancellationToken = default)
        {
            var weapons = await _context.Weapons
                .Include(w => w.WeaponProfiles)
                    .ThenInclude(wp => wp.WeaponProfileAbilities)
                        .ThenInclude(wpa => wpa.Ability)
                .OrderBy(w => w.Name)
                .ToListAsync(cancellationToken);

            var result = new List<WeaponListItemViewModel>();

            foreach (var weapon in weapons)
            {
                result.Add(await BuildWeaponListItemAsync(weapon, cancellationToken));
            }

            return result;
        }

        public async Task<WeaponListItemViewModel?> GetWeaponAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var weapon = await _context.Weapons
                .Include(w => w.WeaponProfiles)
                    .ThenInclude(wp => wp.WeaponProfileAbilities)
                        .ThenInclude(wpa => wpa.Ability)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (weapon == null)
            {
                return null;
            }

            return await BuildWeaponListItemAsync(weapon, cancellationToken);
        }

        public async Task<EditWeaponPageInput?> GetWeaponForEditAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var weapon = await _context.Weapons
                .Include(w => w.WeaponProfiles)
                    .ThenInclude(wp => wp.WeaponProfileAbilities)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (weapon == null)
            {
                return null;
            }

            return new EditWeaponPageInput
            {
                Name = weapon.Name,
                Profiles = weapon.WeaponProfiles
                    .OrderBy(p => p.Id)
                    .Select(p => new EditWeaponProfilePageInput
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Range = p.Range,
                        Attacks = p.Attacks,
                        Skill = p.Skill,
                        Strength = p.Strength,
                        ArmorPiercing = p.ArmorPiercing,
                        Damage = p.Damage,
                        AbilityIds = p.WeaponProfileAbilities
                            .Select(a => a.AbilityId)
                            .ToList()
                    })
                    .ToList()
            };
        }

        public async Task<List<SelectListItem>> GetAbilitySelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Abilities
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CreateWeaponAsync(
            CreateWeaponPageInput input,
            CancellationToken cancellationToken = default)
        {
            var weapon = new Weapon
            {
                Name = input.Name.Trim(),
                WeaponProfiles = input.Profiles
                    .Where(IsCreateProfileFilled)
                    .Select(p => new WeaponProfile
                    {
                        Name = string.IsNullOrWhiteSpace(p.Name) ? null : p.Name.Trim(),
                        Range = p.Range,
                        Attacks = p.Attacks.Trim(),
                        Skill = p.Skill,
                        Strength = p.Strength,
                        ArmorPiercing = p.ArmorPiercing,
                        Damage = p.Damage.Trim(),
                        WeaponProfileAbilities = (p.AbilityIds ?? new List<int>())
                        .Distinct()
                        .Select(id => new WeaponProfileAbility
                        {
                            AbilityId = id
                        })
                        .ToList()
                    })
                    .ToList()
            };

            if (!weapon.WeaponProfiles.Any())
            {
                throw new InvalidOperationException("Нужно добавить хотя бы один профиль оружия.");
            }

            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync(cancellationToken);

            return weapon.Id;
        }

        public async Task<bool> UpdateWeaponAsync(
            int id,
            EditWeaponPageInput input,
            CancellationToken cancellationToken = default)
        {
            var weapon = await _context.Weapons
                .Include(w => w.WeaponProfiles)
                    .ThenInclude(wp => wp.WeaponProfileAbilities)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (weapon == null)
            {
                return false;
            }

            weapon.Name = input.Name.Trim();

            foreach (var inputProfile in input.Profiles)
            {
                var profile = weapon.WeaponProfiles
                    .FirstOrDefault(p => p.Id == inputProfile.Id);

                if (profile == null)
                {
                    continue;
                }

                profile.Name = string.IsNullOrWhiteSpace(inputProfile.Name)
                    ? null
                    : inputProfile.Name.Trim();

                profile.Range = inputProfile.Range;
                profile.Attacks = inputProfile.Attacks.Trim();
                profile.Skill = inputProfile.Skill;
                profile.Strength = inputProfile.Strength;
                profile.ArmorPiercing = inputProfile.ArmorPiercing;
                profile.Damage = inputProfile.Damage.Trim();

                var selectedAbilityIds = (inputProfile.AbilityIds ?? new List<int>())
                .Distinct()
                .ToHashSet();

                var existingAbilityLinks = profile.WeaponProfileAbilities.ToList();

                foreach (var existing in existingAbilityLinks)
                {
                    if (!selectedAbilityIds.Contains(existing.AbilityId))
                    {
                        _context.WeaponProfileAbilities.Remove(existing);
                    }
                }

                var existingIds = existingAbilityLinks
                    .Select(x => x.AbilityId)
                    .ToHashSet();

                foreach (var abilityId in selectedAbilityIds)
                {
                    if (!existingIds.Contains(abilityId))
                    {
                        profile.WeaponProfileAbilities.Add(new WeaponProfileAbility
                        {
                            WeaponProfileId = profile.Id,
                            AbilityId = abilityId
                        });
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<WeaponDeleteResult> DeleteWeaponAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var weapon = await GetWeaponAsync(id, cancellationToken);

            if (weapon == null)
            {
                return new WeaponDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Оружие не найдено."
                };
            }

            if (!weapon.CanDelete)
            {
                return new WeaponDeleteResult
                {
                    Success = false,
                    ErrorMessage = BuildDeleteBlockedMessage(weapon)
                };
            }

            var entity = await _context.Weapons
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (entity == null)
            {
                return new WeaponDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Оружие не найдено."
                };
            }

            _context.Weapons.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new WeaponDeleteResult
            {
                Success = true
            };
        }

        private async Task<WeaponListItemViewModel> BuildWeaponListItemAsync(
            Weapon weapon,
            CancellationToken cancellationToken)
        {
            var usedByModels = await _context.ModelWeapons
                .Where(mw => mw.WeaponId == weapon.Id)
                .Include(mw => mw.Model)
                .Where(mw => mw.Model != null)
                .Select(mw => mw.Model!.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync(cancellationToken);

            var simulationResultUsageCount = await _context.WeaponStats
                .CountAsync(ws => ws.WeaponId == weapon.Id, cancellationToken);

            return new WeaponListItemViewModel
            {
                Id = weapon.Id,
                Name = weapon.Name,
                UsedByModels = usedByModels,
                SimulationResultUsageCount = simulationResultUsageCount,
                Profiles = weapon.WeaponProfiles
                    .OrderBy(p => p.Id)
                    .Select(p => new WeaponProfileListItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Range = p.Range,
                        Attacks = p.Attacks,
                        Skill = p.Skill,
                        Strength = p.Strength,
                        ArmorPiercing = p.ArmorPiercing,
                        Damage = p.Damage,
                        Abilities = p.WeaponProfileAbilities
                            .Where(a => a.Ability != null)
                            .Select(a => a.Ability!.Name)
                            .OrderBy(name => name)
                            .ToList()
                    })
                    .ToList()
            };
        }

        private static bool IsCreateProfileFilled(CreateWeaponProfilePageInput profile)
        {
            return !string.IsNullOrWhiteSpace(profile.Attacks) &&
                   !string.IsNullOrWhiteSpace(profile.Damage);
        }

        private static string BuildDeleteBlockedMessage(WeaponListItemViewModel weapon)
        {
            var parts = new List<string>();

            if (weapon.IsUsedByModels)
            {
                parts.Add($"используется моделями: {string.Join(", ", weapon.UsedByModels)}");
            }

            if (weapon.IsUsedBySimulationResults)
            {
                parts.Add($"используется в сохранённых результатах симуляций: {weapon.SimulationResultUsageCount}");
            }

            return "Оружие не может быть удалено, потому что " + string.Join("; ", parts) + ".";
        }
    }
}