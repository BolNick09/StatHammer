using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatHammer.Server.PageServices.Simulations;
using StatHammer.Server.Simulation.Models;
using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.Pages.Simulations
{
    public class IndexModel : PageModel
    {
        private readonly ISimulationPageService _simulationPageService;

        public IndexModel(ISimulationPageService simulationPageService)
        {
            _simulationPageService = simulationPageService;
        }

        [BindProperty]
        public SimulationInputModel Input { get; set; } = new();

        public List<SelectListItem> Units { get; set; } = new();

        public SimulationRunViewModel? RunResult { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Units = await _simulationPageService.GetUnitSelectListAsync(cancellationToken);
            EnsureDefaultUnitSelection();
            await LoadSelectedUnitLoadoutsAsync(cancellationToken);
        }

        public async Task<IActionResult> OnPostLoadoutAsync(CancellationToken cancellationToken)
        {
            Units = await _simulationPageService.GetUnitSelectListAsync(cancellationToken);
            EnsureDefaultUnitSelection();

            await RefreshLoadoutsAfterUnitSelectionChangeAsync(cancellationToken);

            return Page();
        }

        private async Task RefreshLoadoutsAfterUnitSelectionChangeAsync(CancellationToken cancellationToken)
        {
            var unitAChanged = Input.UnitALoadoutUnitId != Input.UnitAId;
            var unitBChanged = Input.UnitBLoadoutUnitId != Input.UnitBId;

            if (unitAChanged || !Input.UnitALoadout.Any())
            {
                ClearUnitALoadoutModelState();

                Input.UnitALoadout = Input.UnitAId > 0
                    ? await _simulationPageService.GetUnitLoadoutAsync(Input.UnitAId, cancellationToken)
                    : new List<SimulationUnitModelCountViewModel>();

                Input.UnitALoadoutUnitId = Input.UnitAId;
            }

            if (unitBChanged || !Input.UnitBLoadout.Any())
            {
                ClearUnitBLoadoutModelState();

                Input.UnitBLoadout = Input.UnitBId > 0
                    ? await _simulationPageService.GetUnitLoadoutAsync(Input.UnitBId, cancellationToken)
                    : new List<SimulationUnitModelCountViewModel>();

                Input.UnitBLoadoutUnitId = Input.UnitBId;
            }
        }

        private void ClearUnitALoadoutModelState()
        {
            foreach (var key in ModelState.Keys
                .Where(key =>
                    key.StartsWith("Input.UnitALoadout") ||
                    key == "Input.UnitALoadoutUnitId")
                .ToList())
            {
                ModelState.Remove(key);
            }
        }

        private void ClearUnitBLoadoutModelState()
        {
            foreach (var key in ModelState.Keys
                .Where(key =>
                    key.StartsWith("Input.UnitBLoadout") ||
                    key == "Input.UnitBLoadoutUnitId")
                .ToList())
            {
                ModelState.Remove(key);
            }
        }

        private void ClearLoadoutModelState()
        {
            foreach (var key in ModelState.Keys
                .Where(key =>
                    key.StartsWith("Input.UnitALoadout") ||
                    key.StartsWith("Input.UnitBLoadout") ||
                    key == "Input.UnitALoadoutUnitId" ||
                    key == "Input.UnitBLoadoutUnitId")
                .ToList())
            {
                ModelState.Remove(key);
            }
        }

        public async Task<IActionResult> OnPostRunAsync(CancellationToken cancellationToken)
        {
            Units = await _simulationPageService.GetUnitSelectListAsync(cancellationToken);

            if (!ModelState.IsValid)
            {
                await EnsureLoadoutsForRedisplayAsync(cancellationToken);
                return Page();
            }

            if (Input.UnitAId == Input.UnitBId)
            {
                ModelState.AddModelError(string.Empty, "Выберите разные юниты для стороны A и стороны B.");
                await EnsureLoadoutsForRedisplayAsync(cancellationToken);
                return Page();
            }

            if (!IsLoadoutActualForSelectedUnits())
            {
                ClearLoadoutModelState();

                await LoadSelectedUnitLoadoutsAsync(cancellationToken);

                ModelState.AddModelError(
                    string.Empty,
                    "Состав юнитов был обновлён после смены выбранных юнитов. Проверьте количество моделей и запустите симуляцию ещё раз.");

                return Page();
            }

            var modifiers = new SimulationModifiers
            {
                UnitA = new UnitCombatModifiers
                {
                    HitModifier = Input.UnitAHitModifier,
                    WoundModifier = Input.UnitAWoundModifier,
                    ArmorPiercingModifier = Input.UnitAArmorPiercingModifier,
                    SaveModifier = Input.UnitASaveModifier
                },
                UnitB = new UnitCombatModifiers
                {
                    HitModifier = Input.UnitBHitModifier,
                    WoundModifier = Input.UnitBWoundModifier,
                    ArmorPiercingModifier = Input.UnitBArmorPiercingModifier,
                    SaveModifier = Input.UnitBSaveModifier
                }
            };

            var loadout = new SimulationLoadout
            {
                UnitA = new UnitLoadoutSelection
                {
                    ModelCounts = Input.UnitALoadout
                        .Select(x => new UnitModelCountSelection
                        {
                            ModelId = x.ModelId,
                            Count = x.Count
                        })
                        .ToList()
                },
                UnitB = new UnitLoadoutSelection
                {
                    ModelCounts = Input.UnitBLoadout
                        .Select(x => new UnitModelCountSelection
                        {
                            ModelId = x.ModelId,
                            Count = x.Count
                        })
                        .ToList()
                }
            };

            try
            {
                RunResult = await _simulationPageService.RunSimulationAsync(
                    Input.UnitAId,
                    Input.UnitBId,
                    Input.SimulationCount,
                    Input.MaxTurns,
                    Input.UseParallel,
                    Input.MaxDegreeOfParallelism,
                    Input.SaveResult,
                    modifiers,
                    loadout,
                    cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            await EnsureLoadoutsForRedisplayAsync(cancellationToken);

            return Page();
        }

        private bool IsLoadoutActualForSelectedUnits()
        {
            return Input.UnitALoadoutUnitId == Input.UnitAId &&
                   Input.UnitBLoadoutUnitId == Input.UnitBId;
        }

        private void EnsureDefaultUnitSelection()
        {
            if (Input.UnitAId == 0 && Units.Count > 0)
            {
                Input.UnitAId = int.Parse(Units[0].Value);
            }

            if (Input.UnitBId == 0 && Units.Count > 1)
            {
                Input.UnitBId = int.Parse(Units[1].Value);
            }
        }

        private async Task LoadSelectedUnitLoadoutsAsync(CancellationToken cancellationToken)
        {
            Input.UnitALoadout = Input.UnitAId > 0
                ? await _simulationPageService.GetUnitLoadoutAsync(Input.UnitAId, cancellationToken)
                : new List<SimulationUnitModelCountViewModel>();

            Input.UnitBLoadout = Input.UnitBId > 0
                ? await _simulationPageService.GetUnitLoadoutAsync(Input.UnitBId, cancellationToken)
                : new List<SimulationUnitModelCountViewModel>();

            Input.UnitALoadoutUnitId = Input.UnitAId;
            Input.UnitBLoadoutUnitId = Input.UnitBId;
        }

        private async Task EnsureLoadoutsForRedisplayAsync(CancellationToken cancellationToken)
        {
            if (!Input.UnitALoadout.Any() ||
                !Input.UnitBLoadout.Any() ||
                !IsLoadoutActualForSelectedUnits())
            {
                ClearLoadoutModelState();
                await LoadSelectedUnitLoadoutsAsync(cancellationToken);
            }
        }

        public class SimulationInputModel
        {
            [Required]
            [Display(Name = "Юнит A")]
            public int UnitAId { get; set; }

            [Required]
            [Display(Name = "Юнит B")]
            public int UnitBId { get; set; }

            [Range(1, 100000)]
            [Display(Name = "Количество симуляций")]
            public int SimulationCount { get; set; } = 1000;

            [Range(1, 5)]
            [Display(Name = "Максимум ходов")]
            public int MaxTurns { get; set; } = 5;

            [Display(Name = "Использовать многопоточность")]
            public bool UseParallel { get; set; } = true;

            [Range(1, 64)]
            [Display(Name = "Количество потоков")]
            public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

            [Display(Name = "Сохранить результат в БД")]
            public bool SaveResult { get; set; } = true;

            public int UnitALoadoutUnitId { get; set; }

            public int UnitBLoadoutUnitId { get; set; }

            public List<SimulationUnitModelCountViewModel> UnitALoadout { get; set; } = new();

            public List<SimulationUnitModelCountViewModel> UnitBLoadout { get; set; } = new();

            [Range(-3, 3)]
            [Display(Name = "Hit")]
            public int UnitAHitModifier { get; set; }

            [Range(-3, 3)]
            [Display(Name = "Wound")]
            public int UnitAWoundModifier { get; set; }

            [Range(-3, 3)]
            [Display(Name = "AP")]
            public int UnitAArmorPiercingModifier { get; set; }

            [Range(-3, 3)]
            [Display(Name = "Save")]
            public int UnitASaveModifier { get; set; }

            [Range(-3, 3)]
            [Display(Name = "Hit")]
            public int UnitBHitModifier { get; set; }

            [Range(-3, 3)]
            [Display(Name = "Wound")]
            public int UnitBWoundModifier { get; set; }

            [Range(-3, 3)]
            [Display(Name = "AP")]
            public int UnitBArmorPiercingModifier { get; set; }

            [Range(-3, 3)]
            [Display(Name = "Save")]
            public int UnitBSaveModifier { get; set; }
        }

        
    }
}