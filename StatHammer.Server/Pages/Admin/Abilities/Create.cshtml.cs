using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Abilities;

namespace StatHammer.Server.Pages.Admin.Abilities
{
    public class CreateModel : PageModel
    {
        private readonly IAbilityAdminPageService _abilityService;

        public CreateModel(IAbilityAdminPageService abilityService)
        {
            _abilityService = abilityService;
        }

        [BindProperty]
        public AbilityPageInput Input { get; set; } = new();

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
                await _abilityService.CreateAbilityAsync(Input, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("/Admin/Abilities/Index");
        }
    }
}