using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Wargears;

namespace StatHammer.Server.Pages.Admin.Wargears
{
    public class EditModel : PageModel
    {
        private readonly IWargearAdminPageService _wargearService;

        public EditModel(IWargearAdminPageService wargearService)
        {
            _wargearService = wargearService;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public WargearPageInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            var wargear = await _wargearService.GetWargearAsync(id, cancellationToken);

            if (wargear == null)
            {
                return NotFound();
            }

            Id = wargear.Id;
            Input = new WargearPageInput
            {
                Name = wargear.Name
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var updated = await _wargearService.UpdateWargearAsync(Id, Input, cancellationToken);

                if (!updated)
                {
                    return NotFound();
                }
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