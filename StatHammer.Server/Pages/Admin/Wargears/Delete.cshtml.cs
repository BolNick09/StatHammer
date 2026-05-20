using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Wargears;

namespace StatHammer.Server.Pages.Admin.Wargears
{
    public class DeleteModel : PageModel
    {
        private readonly IWargearAdminPageService _wargearService;

        public DeleteModel(IWargearAdminPageService wargearService)
        {
            _wargearService = wargearService;
        }

        [BindProperty]
        public int Id { get; set; }

        public WargearListItemViewModel? Wargear { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            Wargear = await _wargearService.GetWargearAsync(id, cancellationToken);

            if (Wargear == null)
            {
                return NotFound();
            }

            Id = id;

            if (!Wargear.CanDelete)
            {
                ErrorMessage =
                    $"Варгир не может быть удалён, потому что используется моделями: " +
                    $"{string.Join(", ", Wargear.UsedByModels)}.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var result = await _wargearService.DeleteWargearAsync(Id, cancellationToken);

            if (!result.Success)
            {
                Wargear = await _wargearService.GetWargearAsync(Id, cancellationToken);
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            return RedirectToPage("/Admin/Wargears/Index");
        }
    }
}