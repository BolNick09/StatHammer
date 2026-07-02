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
            var unitModels = await _context.UnitModels
                .AsNoTracking()
                .Include(um => um.Model)
                    .ThenInclude(m => m!.ModelWeapons)
                        .ThenInclude(mw => mw.Weapon)
                            .ThenInclude(w => w!.WeaponProfiles)
                                .ThenInclude(wp => wp.WeaponProfileAbilities)
                                    .ThenInclude(wpa => wpa.Ability)
                .Where(um => um.UnitId == unitId)
                .Where(um => um.Model != null)
                .OrderBy(um => um.Model!.Name)
                .ToListAsync(cancellationToken);

            return unitModels
                .Select(um => new SimulationUnitModelCountViewModel
                {
                    ModelId = um.ModelId,
                    ModelName = um.Model!.Name,
                    MinCount = um.MinCount,
                    MaxCount = um.MaxCount,
                    Count = um.MinCount,

                    Move = um.Model.Move,
                    Toughness = um.Model.Toughness,
                    Save = um.Model.Save,
                    InvulnerableSave = um.Model.InvulnerableSave,
                    Wounds = um.Model.Wounds,
                    Leadership = um.Model.Leadership,
                    OC = um.Model.OC,

                    WeaponProfiles = um.Model.ModelWeapons
                        .Where(mw => mw.Weapon != null)
                        .OrderBy(mw => mw.Weapon!.Name)
                        .SelectMany(mw => mw.Weapon!.WeaponProfiles
                            .OrderBy(wp => wp.Name)
                            .Select(wp => new SimulationUnitWeaponProfileViewModel
                            {
                                WeaponName = mw.Weapon!.Name,
                                ProfileName = wp.Name,
                                Range = wp.Range,
                                Attacks = wp.Attacks,
                                Skill = wp.Skill,
                                Strength = wp.Strength,
                                ArmorPiercing = wp.ArmorPiercing,
                                Damage = wp.Damage,
                                Abilities = wp.WeaponProfileAbilities
                                    .Where(wpa => wpa.Ability != null)
                                    .Select(wpa => wpa.Ability!.Name)
                                    .OrderBy(name => name)
                                    .ToList()
                            }))
                        .ToList()
                })
                .ToList();
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