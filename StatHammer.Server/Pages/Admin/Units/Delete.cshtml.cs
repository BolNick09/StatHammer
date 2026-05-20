using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Units;

namespace StatHammer.Server.Pages.Admin.Units
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitAdminPageService _unitService;

        public DeleteModel(IUnitAdminPageService unitService)
        {
            _unitService = unitService;
        }

        [BindProperty]
        public int Id { get; set; }

        public UnitListItemViewModel? Unit { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            Unit = await _unitService.GetUnitAsync(id, cancellationToken);

            if (Unit == null)
            {
                return NotFound();
            }

            Id = id;

            if (!Unit.CanDelete)
            {
                ErrorMessage =
                    $"ёнит не может быть удалЄн, потому что используетс€ в сохранЄнных результатах симул€ций: " +
                    $"{Unit.SimulationResultUsageCount}.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var result = await _unitService.DeleteUnitAsync(Id, cancellationToken);

            if (!result.Success)
            {
                Unit = await _unitService.GetUnitAsync(Id, cancellationToken);
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            return RedirectToPage("/Admin/Units/Index");
        }
    }
}