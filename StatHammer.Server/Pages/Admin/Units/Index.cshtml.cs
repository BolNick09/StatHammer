using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Units;

namespace StatHammer.Server.Pages.Admin.Units
{
    public class IndexModel : PageModel
    {
        private readonly IUnitAdminPageService _unitService;

        public IndexModel(IUnitAdminPageService unitService)
        {
            _unitService = unitService;
        }

        public List<UnitListItemViewModel> Units { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Units = await _unitService.GetUnitsAsync(cancellationToken);
        }
    }
}