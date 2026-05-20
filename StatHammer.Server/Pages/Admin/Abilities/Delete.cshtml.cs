using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Abilities;

namespace StatHammer.Server.Pages.Admin.Abilities
{
    public class DeleteModel : PageModel
    {
        private readonly IAbilityAdminPageService _abilityService;

        public DeleteModel(IAbilityAdminPageService abilityService)
        {
            _abilityService = abilityService;
        }

        [BindProperty]
        public int Id { get; set; }

        public AbilityListItemViewModel? Ability { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            Ability = await _abilityService.GetAbilityAsync(id, cancellationToken);

            if (Ability == null)
            {
                return NotFound();
            }

            Id = id;

            if (!Ability.CanDelete)
            {
                ErrorMessage =
                    $"Правило используется и не может быть удалено. " +
                    $"Профили оружия: {Ability.WeaponProfileUsageCount}, " +
                    $"модели: {Ability.ModelUsageCount}, " +
                    $"юниты: {Ability.UnitUsageCount}.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var result = await _abilityService.DeleteAbilityAsync(Id, cancellationToken);

            if (!result.Success)
            {
                Ability = await _abilityService.GetAbilityAsync(Id, cancellationToken);
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            return RedirectToPage("/Admin/Abilities/Index");
        }
    }
}