using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.PageServices.Admin.Abilities
{
    public class AbilityAdminPageService : IAbilityAdminPageService
    {
        private readonly StatHammerDbContext _context;

        public AbilityAdminPageService(StatHammerDbContext context)
        {
            _context = context;
        }

        public async Task<List<AbilityListItemViewModel>> GetAbilitiesAsync(
            CancellationToken cancellationToken = default)
        {
            var abilities = await _context.Abilities
                .OrderBy(a => a.Name)
                .Select(a => new AbilityListItemViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    WeaponProfileUsageCount = _context.WeaponProfileAbilities
                        .Count(wpa => wpa.AbilityId == a.Id),
                    ModelUsageCount = _context.ModelAbilities
                        .Count(ma => ma.AbilityId == a.Id),
                    UnitUsageCount = _context.UnitAbilities
                        .Count(ua => ua.AbilityId == a.Id)
                })
                .ToListAsync(cancellationToken);

            return abilities;
        }

        public async Task<AbilityListItemViewModel?> GetAbilityAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Abilities
                .Where(a => a.Id == id)
                .Select(a => new AbilityListItemViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    WeaponProfileUsageCount = _context.WeaponProfileAbilities
                        .Count(wpa => wpa.AbilityId == a.Id),
                    ModelUsageCount = _context.ModelAbilities
                        .Count(ma => ma.AbilityId == a.Id),
                    UnitUsageCount = _context.UnitAbilities
                        .Count(ua => ua.AbilityId == a.Id)
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CreateAbilityAsync(
            AbilityPageInput input,
            CancellationToken cancellationToken = default)
        {
            var normalizedName = input.Name.Trim();

            var duplicateExists = await _context.Abilities
                .AnyAsync(a => a.Name == normalizedName, cancellationToken);

            if (duplicateExists)
            {
                throw new InvalidOperationException("Правило с таким названием уже существует.");
            }

            var ability = new Ability
            {
                Name = normalizedName
            };

            _context.Abilities.Add(ability);
            await _context.SaveChangesAsync(cancellationToken);

            return ability.Id;
        }

        public async Task<bool> UpdateAbilityAsync(
            int id,
            AbilityPageInput input,
            CancellationToken cancellationToken = default)
        {
            var ability = await _context.Abilities
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (ability == null)
            {
                return false;
            }

            var normalizedName = input.Name.Trim();

            var duplicateExists = await _context.Abilities
                .AnyAsync(a => a.Id != id && a.Name == normalizedName, cancellationToken);

            if (duplicateExists)
            {
                throw new InvalidOperationException("Правило с таким названием уже существует.");
            }

            ability.Name = normalizedName;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<AbilityDeleteResult> DeleteAbilityAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var ability = await GetAbilityAsync(id, cancellationToken);

            if (ability == null)
            {
                return new AbilityDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Правило не найдено."
                };
            }

            if (!ability.CanDelete)
            {
                return new AbilityDeleteResult
                {
                    Success = false,
                    ErrorMessage =
                        $"Правило используется и не может быть удалено. " +
                        $"Профили оружия: {ability.WeaponProfileUsageCount}, " +
                        $"модели: {ability.ModelUsageCount}, " +
                        $"юниты: {ability.UnitUsageCount}."
                };
            }

            var entity = await _context.Abilities
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (entity == null)
            {
                return new AbilityDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Правило не найдено."
                };
            }

            _context.Abilities.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new AbilityDeleteResult
            {
                Success = true
            };
        }
    }
}
