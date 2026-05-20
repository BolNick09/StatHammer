using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Wargears;

namespace StatHammer.Server.Pages.Admin.Wargears
{
    public class CreateModel : PageModel
    {
        private readonly IWargearAdminPageService _wargearService;

        public CreateModel(IWargearAdminPageService wargearService)
        {
            _wargearService = wargearService;
        }

        [BindProperty]
        public WargearPageInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _wargearService.CreateWargearAsync(Input, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("/Admin/Wargears/Index");
        }
    }
}