using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Models;

namespace StatHammer.Server.Pages.Admin.Models
{
    public class DeleteModel : PageModel
    {
        private readonly IModelAdminPageService _modelService;

        public DeleteModel(IModelAdminPageService modelService)
        {
            _modelService = modelService;
        }

        [BindProperty]
        public int Id { get; set; }

        public ModelListItemViewModel? GameModel { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            GameModel = await _modelService.GetModelAsync(id, cancellationToken);

            if (GameModel == null)
            {
                return NotFound();
            }

            Id = id;

            if (!GameModel.CanDelete)
            {
                ErrorMessage =
                    $"Модель не может быть удалена, потому что используется в юнитах: " +
                    $"{string.Join(", ", GameModel.UsedByUnits)}.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var result = await _modelService.DeleteModelAsync(Id, cancellationToken);

            if (!result.Success)
            {
                GameModel = await _modelService.GetModelAsync(Id, cancellationToken);
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            return RedirectToPage("/Admin/Models/Index");
        }
    }
}