using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;
using StatHammer.Server.Simulation.Battle.Services;
using StatHammer.Server.Simulation.Dice.Services;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.PageServices.Simulations
{
    public class SimulationPageService : ISimulationPageService
    {
        private readonly StatHammerDbContext _context;
        private readonly IBattleBatchSimulationService _singleThreadService;
        private readonly IBattleBatchSimulationParallelService _parallelService;
        private readonly IBattleResultPersistenceService _persistenceService;

        public SimulationPageService(
            StatHammerDbContext context,
            IBattleBatchSimulationService singleThreadService,
            IBattleBatchSimulationParallelService parallelService,
            IBattleResultPersistenceService persistenceService)
        {
            _context = context;
            _singleThreadService = singleThreadService;
            _parallelService = parallelService;
            _persistenceService = persistenceService;
        }

        public async Task<List<SelectListItem>> GetUnitSelectListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Units
                .OrderBy(u => u.Name)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<SimulationUnitModelCountViewModel>> GetUnitLoadoutAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await _context.UnitModels
                .AsNoTracking()
                .Include(um => um.Model)
                .Where(um => um.UnitId == unitId)
                .Where(um => um.Model != null)
                .OrderBy(um => um.Model!.Name)
                .Select(um => new SimulationUnitModelCountViewModel
                {
                    ModelId = um.ModelId,
                    ModelName = um.Model!.Name,
                    MinCount = um.MinCount,
                    MaxCount = um.MaxCount,
                    Count = um.MinCount
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<SimulationRunViewModel> RunSimulationAsync(
            int unitAId,
            int unitBId,
            int simulationCount,
            int maxTurns,
            bool useParallel,
            int maxDegreeOfParallelism,
            bool saveResult,
            SimulationModifiers modifiers,
            SimulationLoadout? loadout,
            CancellationToken cancellationToken = default)
        {
            BattleSimulationBatchResult result;

            if (useParallel)
            {
                result = await _parallelService.RunBatchAsync(
                    unitAId,
                    unitBId,
                    simulationCount,
                    maxTurns,
                    unitAPrefersMelee: false,
                    unitBPrefersMelee: false,
                    maxDegreeOfParallelism,
                    cancellationToken,
                    modifiers,
                    loadout);
            }
            else
            {
                result = await _singleThreadService.RunBatchAsync(
                    unitAId,
                    unitBId,
                    simulationCount,
                    maxTurns,
                    unitAPrefersMelee: false,
                    unitBPrefersMelee: false,
                    cancellationToken,
                    modifiers,
                    loadout);
            }

            var viewModel = new SimulationRunViewModel
            {
                Result = result
            };

            if (saveResult)
            {
                var saved = await _persistenceService.SaveBatchResultAsync(
                    unitAId,
                    unitBId,
                    result,
                    cancellationToken);

                viewModel.SavedSimulationResultId = saved.Id;
            }

            return viewModel;
        }
    }
}