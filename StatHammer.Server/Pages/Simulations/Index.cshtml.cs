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
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            Units = await _simulationPageService.GetUnitSelectListAsync(cancellationToken);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.UnitAId == Input.UnitBId)
            {
                ModelState.AddModelError(string.Empty, "Выберите разные юниты для стороны A и стороны B.");
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

            RunResult = await _simulationPageService.RunSimulationAsync(
                Input.UnitAId,
                Input.UnitBId,
                Input.SimulationCount,
                Input.MaxTurns,
                Input.UseParallel,
                Input.MaxDegreeOfParallelism,
                Input.SaveResult,
                modifiers,
                cancellationToken);

            return Page();
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