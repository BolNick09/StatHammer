using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatHammer.Server.PageServices.Admin.Weapons;

namespace StatHammer.Server.Pages.Admin.Weapons
{
    public class EditModel : PageModel
    {
        private readonly IWeaponAdminPageService _weaponService;

        public EditModel(IWeaponAdminPageService weaponService)
        {
            _weaponService = weaponService;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public EditWeaponPageInput Input { get; set; } = new();

        public List<SelectListItem> Abilities { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            Abilities = await _weaponService.GetAbilitySelectListAsync(cancellationToken);

            var weapon = await _weaponService.GetWeaponForEditAsync(id, cancellationToken);

            if (weapon == null)
            {
                return NotFound();
            }

            Id = id;
            Input = weapon;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            Abilities = await _weaponService.GetAbilitySelectListAsync(cancellationToken);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updated = await _weaponService.UpdateWeaponAsync(Id, Input, cancellationToken);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToPage("/Admin/Weapons/Index");
        }
    }
}