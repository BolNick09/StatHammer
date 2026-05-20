using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.PageServices.Admin.Keywords
{
    public class KeywordAdminPageService : IKeywordAdminPageService
    {
        private readonly StatHammerDbContext _context;

        public KeywordAdminPageService(StatHammerDbContext context)
        {
            _context = context;
        }

        public async Task<List<KeywordListItemViewModel>> GetKeywordsAsync(
            CancellationToken cancellationToken = default)
        {
            var keywords = await _context.Keywords
                .OrderBy(k => k.Name)
                .ToListAsync(cancellationToken);

            var result = new List<KeywordListItemViewModel>();

            foreach (var keyword in keywords)
            {
                result.Add(await BuildViewModelAsync(keyword, cancellationToken));
            }

            return result;
        }

        public async Task<KeywordListItemViewModel?> GetKeywordAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var keyword = await _context.Keywords
                .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);

            if (keyword == null)
            {
                return null;
            }

            return await BuildViewModelAsync(keyword, cancellationToken);
        }

        public async Task<int> CreateKeywordAsync(
            KeywordPageInput input,
            CancellationToken cancellationToken = default)
        {
            var normalizedWord = input.Name.Trim();

            var duplicateExists = await _context.Keywords
                .AnyAsync(k => k.Name == normalizedWord, cancellationToken);

            if (duplicateExists)
            {
                throw new InvalidOperationException("Такое ключевое слово уже существует.");
            }

            var keyword = new Keyword
            {
                Name = normalizedWord
            };

            _context.Keywords.Add(keyword);
            await _context.SaveChangesAsync(cancellationToken);

            return keyword.Id;
        }

        public async Task<bool> UpdateKeywordAsync(
            int id,
            KeywordPageInput input,
            CancellationToken cancellationToken = default)
        {
            var keyword = await _context.Keywords
                .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);

            if (keyword == null)
            {
                return false;
            }

            var normalizedWord = input.Name.Trim();

            var duplicateExists = await _context.Keywords
                .AnyAsync(k => k.Id != id && k.Name == normalizedWord, cancellationToken);

            if (duplicateExists)
            {
                throw new InvalidOperationException("Такое ключевое слово уже существует.");
            }

            keyword.Name = normalizedWord;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<KeywordDeleteResult> DeleteKeywordAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var keyword = await GetKeywordAsync(id, cancellationToken);

            if (keyword == null)
            {
                return new KeywordDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Ключевое слово не найдено."
                };
            }

            if (!keyword.CanDelete)
            {
                return new KeywordDeleteResult
                {
                    Success = false,
                    ErrorMessage =
                        $"Ключевое слово не может быть удалено, потому что используется юнитами: " +
                        $"{string.Join(", ", keyword.UsedByUnits)}."
                };
            }

            var entity = await _context.Keywords
                .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);

            if (entity == null)
            {
                return new KeywordDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Ключевое слово не найдено."
                };
            }

            _context.Keywords.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new KeywordDeleteResult
            {
                Success = true
            };
        }

        private async Task<KeywordListItemViewModel> BuildViewModelAsync(
            Keyword keyword,
            CancellationToken cancellationToken)
        {
            var usedByUnits = await _context.UnitKeywords
                .Where(uk => uk.KeywordId == keyword.Id)
                .Include(uk => uk.Unit)
                .Where(uk => uk.Unit != null)
                .Select(uk => uk.Unit!.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync(cancellationToken);

            return new KeywordListItemViewModel
            {
                Id = keyword.Id,
                Name = keyword.Name,
                UsedByUnits = usedByUnits
            };
        }
    }
}