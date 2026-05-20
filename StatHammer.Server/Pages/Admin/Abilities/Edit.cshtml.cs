using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Abilities;

namespace StatHammer.Server.Pages.Admin.Abilities
{
    public class EditModel : PageModel
    {
        private readonly IAbilityAdminPageService _abilityService;

        public EditModel(IAbilityAdminPageService abilityService)
        {
            _abilityService = abilityService;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public AbilityPageInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            var ability = await _abilityService.GetAbilityAsync(id, cancellationToken);

            if (ability == null)
            {
                return NotFound();
            }

            Id = ability.Id;
            Input = new AbilityPageInput
            {
                Name = ability.Name
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
                var updated = await _abilityService.UpdateAbilityAsync(Id, Input, cancellationToken);

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

            return RedirectToPage("/Admin/Abilities/Index");
        }
    }
}