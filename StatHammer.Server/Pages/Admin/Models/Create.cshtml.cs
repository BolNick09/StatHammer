using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatHammer.Server.PageServices.Admin.Models;

namespace StatHammer.Server.Pages.Admin.Models
{
    public class CreateModel : PageModel
    {
        private readonly IModelAdminPageService _modelService;

        public CreateModel(IModelAdminPageService modelService)
        {
            _modelService = modelService;
        }

        [BindProperty]
        public ModelPageInput Input { get; set; } = new();

        public List<SelectListItem> Weapons { get; set; } = new();

        public List<SelectListItem> Wargears { get; set; } = new();


        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            await LoadSelectListsAsync(cancellationToken);
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            await LoadSelectListsAsync(cancellationToken);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _modelService.CreateModelAsync(Input, cancellationToken);

            return RedirectToPage("/Admin/Models/Index");
        }

        private async Task LoadSelectListsAsync(CancellationToken cancellationToken)
        {
            Weapons = await _modelService.GetWeaponSelectListAsync(cancellationToken);
            Wargears = await _modelService.GetWargearSelectListAsync(cancellationToken);
        }
    }
}