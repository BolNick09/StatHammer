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
        public CreateWeaponPageInput Input { get; set; } = CreateDefaultInput();

        public List<SelectListItem> Abilities { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            EnsureTwoProfiles();
            Abilities = await _weaponService.GetAbilitySelectListAsync(cancellationToken);
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            Abilities = await _weaponService.GetAbilitySelectListAsync(cancellationToken);

            EnsureTwoProfiles();

            if (!Input.HasSecondProfile)
            {
                RemoveSecondProfileModelState();
            }

            Input.Profiles = Input.HasSecondProfile
                ? Input.Profiles.Take(2).ToList()
                : Input.Profiles.Take(1).ToList();

            if (!Input.Profiles.Any())
            {
                ModelState.AddModelError(string.Empty, "Нужно добавить хотя бы один профиль оружия.");
            }

            if (!ModelState.IsValid)
            {
                EnsureTwoProfiles();
                return Page();
            }

            try
            {
                await _weaponService.CreateWeaponAsync(Input, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                EnsureTwoProfiles();
                return Page();
            }

            return RedirectToPage("/Admin/Weapons/Index");
        }

        private static CreateWeaponPageInput CreateDefaultInput()
        {
            return new CreateWeaponPageInput
            {
                Profiles = new List<CreateWeaponProfilePageInput>
                {
                    new CreateWeaponProfilePageInput(),
                    new CreateWeaponProfilePageInput()
                }
            };
        }

        private void EnsureTwoProfiles()
        {
            Input.Profiles ??= new List<CreateWeaponProfilePageInput>();

            while (Input.Profiles.Count < 2)
            {
                Input.Profiles.Add(new CreateWeaponProfilePageInput());
            }
        }

        private void RemoveSecondProfileModelState()
        {
            foreach (var key in ModelState.Keys
                .Where(key => key.StartsWith("Input.Profiles[1]."))
                .ToList())
            {
                ModelState.Remove(key);
            }
        }
    }
}