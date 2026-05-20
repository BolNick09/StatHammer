using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatHammer.Server.PageServices.Admin.Models;

namespace StatHammer.Server.Pages.Admin.Models
{
    public class EditModel : PageModel
    {
        private readonly IModelAdminPageService _modelService;

        public EditModel(IModelAdminPageService modelService)
        {
            _modelService = modelService;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public ModelPageInput Input { get; set; } = new();

        public List<SelectListItem> Weapons { get; set; } = new();

        public List<SelectListItem> Wargears { get; set; } = new();


        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            await LoadSelectListsAsync(cancellationToken);

            var model = await _modelService.GetModelForEditAsync(id, cancellationToken);

            if (model == null)
            {
                return NotFound();
            }

            Id = id;
            Input = model;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            await LoadSelectListsAsync(cancellationToken);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updated = await _modelService.UpdateModelAsync(Id, Input, cancellationToken);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToPage("/Admin/Models/Index");
        }

        private async Task LoadSelectListsAsync(CancellationToken cancellationToken)
        {
            Weapons = await _modelService.GetWeaponSelectListAsync(cancellationToken);
            Wargears = await _modelService.GetWargearSelectListAsync(cancellationToken);
        }
    }
}