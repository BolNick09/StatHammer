using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Simulation.Battle.Models.Aggregation;
using StatHammer.Server.Simulation.Battle.Services;
using StatHammer.Server.Simulation.Dice.Services;

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

        public async Task<List<SelectListItem>> GetUnitSelectListAsync(CancellationToken cancellationToken = default)
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

        public async Task<SimulationRunViewModel> RunSimulationAsync(
            int unitAId,
            int unitBId,
            int simulationCount,
            int maxTurns,
            bool useParallel,
            int maxDegreeOfParallelism,
            bool saveResult,
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
                    cancellationToken);
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
                    cancellationToken);
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
