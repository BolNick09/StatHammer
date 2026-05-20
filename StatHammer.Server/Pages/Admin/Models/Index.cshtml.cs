using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Models;

namespace StatHammer.Server.Pages.Admin.Models
{
    public class IndexModel : PageModel
    {
        private readonly IModelAdminPageService _modelService;

        public IndexModel(IModelAdminPageService modelService)
        {
            _modelService = modelService;
        }

        public List<ModelListItemViewModel> Models { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Models = await _modelService.GetModelsAsync(cancellationToken);
        }
    }
}