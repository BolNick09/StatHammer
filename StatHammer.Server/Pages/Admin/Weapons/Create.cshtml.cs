using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatHammer.Server.PageServices.Admin.Weapons;

namespace StatHammer.Server.Pages.Admin.Weapons
{
    public class CreateModel : PageModel
    {
        private readonly IWeaponAdminPageService _weaponService;

        public CreateModel(IWeaponAdminPageService weaponService)
        {
            _weaponService = weaponService;
        }

        [BindProperty]
        public CreateWeaponPageInput Input { get; set; } = new()
        {
            Profiles = new List<CreateWeaponProfilePageInput>
            {
                new CreateWeaponProfilePageInput(),
                new CreateWeaponProfilePageInput()
            }
        };

        public List<SelectListItem> Abilities { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Abilities = await _weaponService.GetAbilitySelectListAsync(cancellationToken);
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            Abilities = await _weaponService.GetAbilitySelectListAsync(cancellationToken);

            Input.Profiles = Input.Profiles
            .Where(p =>
                !string.IsNullOrWhiteSpace(p.Name) ||
                !string.IsNullOrWhiteSpace(p.Attacks) ||
                !string.IsNullOrWhiteSpace(p.Damage) ||
                p.Range != 0 ||
                p.Strength != 4 ||
                p.ArmorPiercing != 0 ||
                (p.AbilityIds?.Any() == true))
            .ToList();

            if (!Input.Profiles.Any())
            {
                ModelState.AddModelError(string.Empty, "Нужно добавить хотя бы один профиль оружия.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _weaponService.CreateWeaponAsync(Input, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("/Admin/Weapons/Index");
        }
    }
}