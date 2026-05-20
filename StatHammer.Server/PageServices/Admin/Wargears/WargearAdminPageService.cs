using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.PageServices.Admin.Wargears
{
    public class WargearAdminPageService : IWargearAdminPageService
    {
        private readonly StatHammerDbContext _context;

        public WargearAdminPageService(StatHammerDbContext context)
        {
            _context = context;
        }

        public async Task<List<WargearListItemViewModel>> GetWargearsAsync(
            CancellationToken cancellationToken = default)
        {
            var wargears = await _context.Wargears
                .OrderBy(w => w.Name)
                .ToListAsync(cancellationToken);

            var result = new List<WargearListItemViewModel>();

            foreach (var wargear in wargears)
            {
                result.Add(await BuildViewModelAsync(wargear, cancellationToken));
            }

            return result;
        }

        public async Task<WargearListItemViewModel?> GetWargearAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var wargear = await _context.Wargears
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (wargear == null)
            {
                return null;
            }

            return await BuildViewModelAsync(wargear, cancellationToken);
        }

        public async Task<int> CreateWargearAsync(
            WargearPageInput input,
            CancellationToken cancellationToken = default)
        {
            var normalizedName = input.Name.Trim();

            var duplicateExists = await _context.Wargears
                .AnyAsync(w => w.Name == normalizedName, cancellationToken);

            if (duplicateExists)
            {
                throw new InvalidOperationException("Варгир с таким названием уже существует.");
            }

            var wargear = new Wargear
            {
                Name = normalizedName
            };

            _context.Wargears.Add(wargear);
            await _context.SaveChangesAsync(cancellationToken);

            return wargear.Id;
        }

        public async Task<bool> UpdateWargearAsync(
            int id,
            WargearPageInput input,
            CancellationToken cancellationToken = default)
        {
            var wargear = await _context.Wargears
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (wargear == null)
            {
                return false;
            }

            var normalizedName = input.Name.Trim();

            var duplicateExists = await _context.Wargears
                .AnyAsync(w => w.Id != id && w.Name == normalizedName, cancellationToken);

            if (duplicateExists)
            {
                throw new InvalidOperationException("Варгир с таким названием уже существует.");
            }

            wargear.Name = normalizedName;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<WargearDeleteResult> DeleteWargearAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var wargear = await GetWargearAsync(id, cancellationToken);

            if (wargear == null)
            {
                return new WargearDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Варгир не найден."
                };
            }

            if (!wargear.CanDelete)
            {
                return new WargearDeleteResult
                {
                    Success = false,
                    ErrorMessage =
                        $"Варгир не может быть удалён, потому что используется моделями: " +
                        $"{string.Join(", ", wargear.UsedByModels)}."
                };
            }

            var entity = await _context.Wargears
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (entity == null)
            {
                return new WargearDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Варгир не найден."
                };
            }

            _context.Wargears.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new WargearDeleteResult
            {
                Success = true
            };
        }

        private async Task<WargearListItemViewModel> BuildViewModelAsync(
            Wargear wargear,
            CancellationToken cancellationToken)
        {
            var usedByModels = await _context.ModelWargears
                .Where(mw => mw.WargearId == wargear.Id)
                .Include(mw => mw.Model)
                .Where(mw => mw.Model != null)
                .Select(mw => mw.Model!.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync(cancellationToken);

            return new WargearListItemViewModel
            {
                Id = wargear.Id,
                Name = wargear.Name,
                UsedByModels = usedByModels
            };
        }
    }
}