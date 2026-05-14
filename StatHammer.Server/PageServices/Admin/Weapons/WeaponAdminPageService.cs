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
            return await _context.Weapons
                .Include(w => w.WeaponProfiles)
                .OrderBy(w => w.Name)
                .Select(w => new WeaponListItemViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Profiles = w.WeaponProfiles
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
                            Damage = p.Damage
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<WeaponListItemViewModel?> GetWeaponAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Weapons
                .Include(w => w.WeaponProfiles)
                .Where(w => w.Id == id)
                .Select(w => new WeaponListItemViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Profiles = w.WeaponProfiles
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
                            Damage = p.Damage
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CreateWeaponAsync(
            CreateWeaponPageInput input,
            CancellationToken cancellationToken = default)
        {
            var weapon = new Weapon
            {
                Name = input.Name.Trim(),
                WeaponProfiles = input.Profiles
                    .Where(p => !string.IsNullOrWhiteSpace(p.Attacks) &&
                                !string.IsNullOrWhiteSpace(p.Damage))
                    .Select(p => new WeaponProfile
                    {
                        Name = string.IsNullOrWhiteSpace(p.Name) ? null : p.Name.Trim(),
                        Range = p.Range,
                        Attacks = p.Attacks.Trim(),
                        Skill = p.Skill,
                        Strength = p.Strength,
                        ArmorPiercing = p.ArmorPiercing,
                        Damage = p.Damage.Trim()
                    })
                    .ToList()
            };

            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync(cancellationToken);

            return weapon.Id;
        }

        public async Task<bool> DeleteWeaponAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var weapon = await _context.Weapons
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (weapon == null)
            {
                return false;
            }

            _context.Weapons.Remove(weapon);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
