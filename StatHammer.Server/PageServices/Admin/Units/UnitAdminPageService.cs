using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.PageServices.Admin.Units
{
    public class UnitAdminPageService : IUnitAdminPageService
    {
        private readonly StatHammerDbContext _context;

        public UnitAdminPageService(StatHammerDbContext context)
        {
            _context = context;
        }

        public async Task<List<UnitListItemViewModel>> GetUnitsAsync(
            CancellationToken cancellationToken = default)
        {
            var units = await _context.Units
                .Include(u => u.UnitModels)
                    .ThenInclude(um => um.Model)
                .Include(u => u.UnitKeywords)
                    .ThenInclude(uk => uk.Keyword)
                .OrderBy(u => u.Name)
                .ToListAsync(cancellationToken);

            var result = new List<UnitListItemViewModel>();

            foreach (var unit in units)
            {
                result.Add(await BuildUnitListItemAsync(unit, cancellationToken));
            }

            return result;
        }

        public async Task<UnitListItemViewModel?> GetUnitAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var unit = await _context.Units
                .Include(u => u.UnitModels)
                    .ThenInclude(um => um.Model)
                .Include(u => u.UnitKeywords)
                    .ThenInclude(uk => uk.Keyword)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (unit == null)
            {
                return null;
            }

            return await BuildUnitListItemAsync(unit, cancellationToken);
        }

        public async Task<UnitPageInput?> GetUnitForEditAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var unit = await _context.Units
                .Include(u => u.UnitModels)
                .Include(u => u.UnitKeywords)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (unit == null)
            {
                return null;
            }

            return new UnitPageInput
            {
                Name = unit.Name,
                Models = unit.UnitModels
                    .OrderBy(um => um.Id)
                    .Select(um => new UnitModelCompositionInput
                    {
                        ModelId = um.ModelId,
                        MinCount = um.MinCount,
                        MaxCount = um.MaxCount
                    })
                    .ToList(),
                KeywordIds = unit.UnitKeywords
                    .Select(uk => uk.KeywordId)
                    .ToList()
            };
        }

        public async Task<List<SelectListItem>> GetModelSelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Models
                .OrderBy(m => m.Name)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<SelectListItem>> GetKeywordSelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Keywords
                .OrderBy(k => k.Name)
                .Select(k => new SelectListItem
                {
                    Value = k.Id.ToString(),
                    Text = k.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CreateUnitAsync(
            UnitPageInput input,
            CancellationToken cancellationToken = default)
        {
            var normalizedModels = NormalizeModelInputs(input.Models);

            ValidateModelComposition(normalizedModels);

            var unit = new Unit
            {
                Name = input.Name.Trim(),
                UnitModels = normalizedModels
                    .Select(m => new UnitModel
                    {
                        ModelId = m.ModelId!.Value,
                        MinCount = m.MinCount,
                        MaxCount = m.MaxCount
                    })
                    .ToList(),
                UnitKeywords = (input.KeywordIds ?? new List<int>())
                    .Distinct()
                    .Select(id => new UnitKeyword
                    {
                        KeywordId = id
                    })
                    .ToList()
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync(cancellationToken);

            return unit.Id;
        }

        public async Task<bool> UpdateUnitAsync(
            int id,
            UnitPageInput input,
            CancellationToken cancellationToken = default)
        {
            var unit = await _context.Units
                .Include(u => u.UnitModels)
                .Include(u => u.UnitKeywords)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (unit == null)
            {
                return false;
            }

            var normalizedModels = NormalizeModelInputs(input.Models);

            ValidateModelComposition(normalizedModels);

            unit.Name = input.Name.Trim();

            ReplaceUnitModels(unit, normalizedModels);
            ReplaceUnitKeywords(unit, input.KeywordIds ?? new List<int>());

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<UnitDeleteResult> DeleteUnitAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var unit = await GetUnitAsync(id, cancellationToken);

            if (unit == null)
            {
                return new UnitDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Юнит не найден."
                };
            }

            if (!unit.CanDelete)
            {
                return new UnitDeleteResult
                {
                    Success = false,
                    ErrorMessage =
                        $"Юнит не может быть удалён, потому что используется в сохранённых результатах симуляций: " +
                        $"{unit.SimulationResultUsageCount}."
                };
            }

            var entity = await _context.Units
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (entity == null)
            {
                return new UnitDeleteResult
                {
                    Success = false,
                    ErrorMessage = "Юнит не найден."
                };
            }

            _context.Units.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new UnitDeleteResult
            {
                Success = true
            };
        }

        private async Task<UnitListItemViewModel> BuildUnitListItemAsync(
            Unit unit,
            CancellationToken cancellationToken)
        {
            var simulationResultUsageCount = await _context.SimulationResults
                .CountAsync(sr => sr.UnitAId == unit.Id || sr.UnitBId == unit.Id, cancellationToken);

            return new UnitListItemViewModel
            {
                Id = unit.Id,
                Name = unit.Name,
                SimulationResultUsageCount = simulationResultUsageCount,
                Models = unit.UnitModels
                    .Where(um => um.Model != null)
                    .OrderBy(um => um.Model!.Name)
                    .Select(um => new UnitModelCompositionViewModel
                    {
                        ModelId = um.ModelId,
                        ModelName = um.Model!.Name,
                        MinCount = um.MinCount,
                        MaxCount = um.MaxCount
                    })
                    .ToList(),
                Keywords = unit.UnitKeywords
                    .Where(uk => uk.Keyword != null)
                    .Select(uk => uk.Keyword!.Name)
                    .OrderBy(name => name)
                    .ToList()
            };
        }

        private static List<UnitModelCompositionInput> NormalizeModelInputs(
            List<UnitModelCompositionInput> inputModels)
        {
            return inputModels
                .Where(m => m.ModelId.HasValue && (m.MinCount > 0 || m.MaxCount > 0))
                .GroupBy(m => m.ModelId!.Value)
                .Select(g =>
                {
                    var first = g.First();

                    return new UnitModelCompositionInput
                    {
                        ModelId = g.Key,
                        MinCount = first.MinCount,
                        MaxCount = first.MaxCount
                    };
                })
                .ToList();
        }

        private static void ValidateModelComposition(List<UnitModelCompositionInput> models)
        {
            if (!models.Any())
            {
                throw new InvalidOperationException("Нужно добавить хотя бы одну модель в состав юнита.");
            }

            foreach (var model in models)
            {
                if (!model.ModelId.HasValue)
                {
                    throw new InvalidOperationException("В составе юнита есть строка без выбранной модели.");
                }

                if (model.MinCount < 0)
                {
                    throw new InvalidOperationException("Минимальное количество моделей не может быть меньше 0.");
                }

                if (model.MaxCount <= 0)
                {
                    throw new InvalidOperationException("Максимальное количество моделей должно быть больше 0.");
                }

                if (model.MinCount > model.MaxCount)
                {
                    throw new InvalidOperationException("Минимальное количество моделей не может быть больше максимального.");
                }
            }
        }

        private void ReplaceUnitModels(Unit unit, List<UnitModelCompositionInput> selectedModels)
        {
            var selectedModelIds = selectedModels
                .Where(m => m.ModelId.HasValue)
                .Select(m => m.ModelId!.Value)
                .ToHashSet();

            foreach (var existing in unit.UnitModels.ToList())
            {
                if (!selectedModelIds.Contains(existing.ModelId))
                {
                    _context.UnitModels.Remove(existing);
                }
            }

            foreach (var selectedModel in selectedModels)
            {
                var modelId = selectedModel.ModelId!.Value;

                var existing = unit.UnitModels
                    .FirstOrDefault(um => um.ModelId == modelId);

                if (existing == null)
                {
                    unit.UnitModels.Add(new UnitModel
                    {
                        UnitId = unit.Id,
                        ModelId = modelId,
                        MinCount = selectedModel.MinCount,
                        MaxCount = selectedModel.MaxCount
                    });
                }
                else
                {
                    existing.MinCount = selectedModel.MinCount;
                    existing.MaxCount = selectedModel.MaxCount;
                }
            }
        }

        private void ReplaceUnitKeywords(Unit unit, List<int> selectedKeywordIds)
        {
            var selected = selectedKeywordIds.Distinct().ToHashSet();

            foreach (var existing in unit.UnitKeywords.ToList())
            {
                if (!selected.Contains(existing.KeywordId))
                {
                    _context.UnitKeywords.Remove(existing);
                }
            }

            var existingIds = unit.UnitKeywords.Select(x => x.KeywordId).ToHashSet();

            foreach (var keywordId in selected)
            {
                if (!existingIds.Contains(keywordId))
                {
                    unit.UnitKeywords.Add(new UnitKeyword
                    {
                        UnitId = unit.Id,
                        KeywordId = keywordId
                    });
                }
            }
        }
    }
}