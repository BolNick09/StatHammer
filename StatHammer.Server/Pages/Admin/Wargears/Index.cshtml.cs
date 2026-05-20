using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Wargears;

namespace StatHammer.Server.Pages.Admin.Wargears
{
    public class IndexModel : PageModel
    {
        private readonly IWargearAdminPageService _wargearService;

        public IndexModel(IWargearAdminPageService wargearService)
        {
            _wargearService = wargearService;
        }

        public List<WargearListItemViewModel> Wargears { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Wargears = await _wargearService.GetWargearsAsync(cancellationToken);
        }
    }
}