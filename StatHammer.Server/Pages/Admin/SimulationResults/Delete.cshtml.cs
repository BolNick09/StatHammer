using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.SimulationResults;

namespace StatHammer.Server.Pages.Admin.SimulationResults
{
    public class DeleteModel : PageModel
    {
        private readonly ISimulationResultAdminPageService _resultService;

        public DeleteModel(ISimulationResultAdminPageService resultService)
        {
            _resultService = resultService;
        }

        [BindProperty]
        public int Id { get; set; }

        public SimulationResultDetailsViewModel? ResultDetails { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            ResultDetails = await _resultService.GetResultDetailsAsync(id, cancellationToken);

            if (ResultDetails == null)
            {
                return NotFound();
            }

            Id = id;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            await _resultService.DeleteResultAsync(Id, cancellationToken);

            return RedirectToPage("/Admin/SimulationResults/Index");
        }
    }
}