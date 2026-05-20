using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Models.Entities;
using EntityModel = StatHammer.Server.Models.Entities.Model;

namespace StatHammer.Server.PageServices.Admin.Models
{
    public class ModelAdminPageService : IModelAdminPageService
    {
        private readonly StatHammerDbContext _context;

        public ModelAdminPageService(StatHammerDbContext context)
        {
            _context = context;
        }

        public async Task<List<ModelListItemViewModel>> GetModelsAsync(
            CancellationToken cancellationToken = default)
        {
            var models = await _context.Models
                .Include(m => m.ModelWeapons)
                    .ThenInclude(mw => mw.Weapon)
                .Include(m => m.ModelWargears)
                    .ThenInclude(mw => mw.Wargear)
                .Include(m => m.ModelAbilities)
                    .ThenInclude(ma => ma.Ability)
                .OrderBy(m => m.Name)
                .ToListAsync(cancellationToken);

            var result = new List<ModelListItemViewModel>();

            foreach (var model in models)
            {
                result.Add(await BuildModelListItemAsync(model, cancellationToken));
            }

            return result;
        }

        public async Task<ModelListItemViewModel?> GetModelAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var model = await _context.Models
                .Include(m => m.ModelWeapons)
                    .ThenInclude(mw => mw.Weapon)
                .Include(m => m.ModelWargears)
                    .ThenInclude(mw => mw.Wargear)
                .Include(m => m.ModelAbilities)
                    .ThenInclude(ma => ma.Ability)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (model == null)
            {
                return null;
            }

            return await BuildModelListItemAsync(model, cancellationToken);
        }

        public async Task<ModelPageInput?> GetModelForEditAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var model = await _context.Models
                .Include(m => m.ModelWeapons)
                .Include(m => m.ModelWargears)
                .Include(m => m.ModelAbilities)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (model == null)
            {
                return null;
            }

            return new ModelPageInput
            {
                Name = model.Name,
                Move = model.Move,
                Toughness = model.Toughness,
                Save = model.Save,
                InvulnerableSave = model.InvulnerableSave,
                Wounds = model.Wounds,
                Leadership = model.Leadership,
                OC = model.OC,
                WeaponIds = model.ModelWeapons.Select(mw => mw.WeaponId).ToList(),
                WargearIds = model.ModelWargears.Select(mw => mw.WargearId).ToList(),
            };
        }

        public async Task<List<SelectListItem>> GetWeaponSelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Weapons
                .OrderBy(w => w.Name)
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<SelectListItem>> GetWargearSelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Wargears
                .OrderBy(w => w.Name)
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CreateModelAsync(
            ModelPageInput input,
            CancellationToken cancellationToken = default)
        {
            var model = new EntityModel
            {
                Name = input.Name.Trim(),
                Move = input.Move,
                Toughness = input.Toughness,
                Save = input.Save,
                InvulnerableSave = input.InvulnerableSave,
                Wounds = input.Wounds,
                Leadership = input.Leadership,
                OC = input.OC,
                ModelWeapons = (input.WeaponIds ?? new List<int>())
                    .Distinct()
                    .Select(id => new ModelWeapon
                    {
                        WeaponId = id,
                        IsDefault = true
                    })
                    .ToList(),
                ModelWargears = (input.WargearIds ?? new List<int>())
                    .Distinct()
                    .Select(id => new ModelWargear
                    {
                        WargearId = id,
                        IsDefault = true
                    })
                    .ToList()
            };

            _context.Models.Add(model);
            await _context.SaveChangesAsync(cancellationToken);

            return model.Id;
        }

        public async Task<bool> UpdateModelAsync(
            int id,
            ModelPageInput input,
            CancellationToken cancellationToken = default)
        {
            var model = await _context.Models
                .Include(m => m.ModelWeapons)
                .Include(m => m.ModelWargears)
                .Include(m => m.ModelAbilities)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (model == null)
            {
                return false;
            }

            model.Name = input.Name.Trim();
            model.Move = input.Move;
            model.Toughness = input.Toughness;
            model.Save = input.Save;
            model.InvulnerableSave = input.InvulnerableSave;
            model.Wounds = input.Wounds;
            model.Leadership = input.Leadership;
            model.OC = input.OC;

            ReplaceWeapons(model, input.WeaponIds ?? new List<int>());
            ReplaceWargears(model, input.WargearIds ?? new List<int>());

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<ModelDeleteResult> DeleteModelAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var model = await GetModelAsync(id, cancellationToken);

            if (model == null)
            {
                return new ModelDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Модель не найдена."
                };
            }

            if (!model.CanDelete)
            {
                return new ModelDeleteResult
                {
                    Success = false,
                    ErrorMessage =
                        $"Модель не может быть удалена, потому что используется в юнитах: " +
                        $"{string.Join(", ", model.UsedByUnits)}."
                };
            }

            var entity = await _context.Models
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (entity == null)
            {
                return new ModelDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Модель не найдена."
                };
            }

            _context.Models.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new ModelDeleteResult
            {
                Success = true
            };
        }

        private async Task<ModelListItemViewModel> BuildModelListItemAsync(
            EntityModel model,
            CancellationToken cancellationToken)
        {
            var usedByUnits = await _context.UnitModels
                .Where(um => um.ModelId == model.Id)
                .Include(um => um.Unit)
                .Where(um => um.Unit != null)
                .Select(um => um.Unit!.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync(cancellationToken);

            return new ModelListItemViewModel
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
                UsedByUnits = usedByUnits,
                Weapons = model.ModelWeapons
                    .Where(mw => mw.Weapon != null)
                    .Select(mw => new ModelEquipmentItemViewModel
                    {
                        Id = mw.WeaponId,
                        Name = mw.Weapon!.Name
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),
                Wargears = model.ModelWargears
                    .Where(mw => mw.Wargear != null)
                    .Select(mw => new ModelEquipmentItemViewModel
                    {
                        Id = mw.WargearId,
                        Name = mw.Wargear!.Name
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),
                Abilities = model.ModelAbilities
                    .Where(ma => ma.Ability != null)
                    .Select(ma => new ModelEquipmentItemViewModel
                    {
                        Id = ma.AbilityId,
                        Name = ma.Ability!.Name
                    })
                    .OrderBy(x => x.Name)
                    .ToList()
            };
        }

        private void ReplaceWeapons(EntityModel model, List<int> selectedIds)
        {
            var selected = selectedIds.Distinct().ToHashSet();

            foreach (var existing in model.ModelWeapons.ToList())
            {
                if (!selected.Contains(existing.WeaponId))
                {
                    _context.ModelWeapons.Remove(existing);
                }
            }

            var existingIds = model.ModelWeapons.Select(x => x.WeaponId).ToHashSet();

            foreach (var weaponId in selected)
            {
                if (!existingIds.Contains(weaponId))
                {
                    model.ModelWeapons.Add(new ModelWeapon
                    {
                        ModelId = model.Id,
                        WeaponId = weaponId,
                        IsDefault = true
                    });
                }
            }
        }

        private void ReplaceWargears(EntityModel model, List<int> selectedIds)
        {
            var selected = selectedIds.Distinct().ToHashSet();

            foreach (var existing in model.ModelWargears.ToList())
            {
                if (!selected.Contains(existing.WargearId))
                {
                    _context.ModelWargears.Remove(existing);
                }
            }

            var existingIds = model.ModelWargears.Select(x => x.WargearId).ToHashSet();

            foreach (var wargearId in selected)
            {
                if (!existingIds.Contains(wargearId))
                {
                    model.ModelWargears.Add(new ModelWargear
                    {
                        ModelId = model.Id,
                        WargearId = wargearId,
                        IsDefault = true
                    });
                }
            }
        }        
    }
}