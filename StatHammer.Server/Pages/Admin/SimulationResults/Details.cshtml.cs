using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.SimulationResults;

namespace StatHammer.Server.Pages.Admin.SimulationResults
{
    public class DetailsModel : PageModel
    {
        private readonly ISimulationResultAdminPageService _resultService;

        public DetailsModel(ISimulationResultAdminPageService resultService)
        {
            _resultService = resultService;
        }

        public SimulationResultDetailsViewModel? ResultDetails { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            ResultDetails = await _resultService.GetResultDetailsAsync(id, cancellationToken);

            if (ResultDetails == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}