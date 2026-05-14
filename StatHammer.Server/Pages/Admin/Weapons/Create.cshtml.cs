using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            Input.Profiles = Input.Profiles
                .Where(p =>
                    !string.IsNullOrWhiteSpace(p.Name) ||
                    !string.IsNullOrWhiteSpace(p.Attacks) ||
                    !string.IsNullOrWhiteSpace(p.Damage) ||
                    p.Range != 0 ||
                    p.Strength != 4 ||
                    p.ArmorPiercing != 0)
                .ToList();

            if (!Input.Profiles.Any())
            {
                ModelState.AddModelError(string.Empty, "Нужно добавить хотя бы один профиль оружия.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var weaponId = await _weaponService.CreateWeaponAsync(Input, cancellationToken);

            return RedirectToPage("/Admin/Weapons/Index");
        }
    }
}