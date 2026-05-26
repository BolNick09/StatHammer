using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.SimulationResults;

namespace StatHammer.Server.Pages.Admin.SimulationResults
{
    public class IndexModel : PageModel
    {
        private readonly ISimulationResultAdminPageService _resultService;

        public IndexModel(ISimulationResultAdminPageService resultService)
        {
            _resultService = resultService;
        }

        public List<SimulationResultListItemViewModel> Results { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Results = await _resultService.GetResultsAsync(cancellationToken);
        }
    }
}