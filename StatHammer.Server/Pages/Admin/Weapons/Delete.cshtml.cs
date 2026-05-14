using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Weapons;

namespace StatHammer.Server.Pages.Admin.Weapons
{
    public class DeleteModel : PageModel
    {
        private readonly IWeaponAdminPageService _weaponService;

        public DeleteModel(IWeaponAdminPageService weaponService)
        {
            _weaponService = weaponService;
        }

        [BindProperty]
        public int Id { get; set; }

        public WeaponListItemViewModel? Weapon { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            Weapon = await _weaponService.GetWeaponAsync(id, cancellationToken);

            if (Weapon == null)
            {
                return NotFound();
            }

            Id = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            await _weaponService.DeleteWeaponAsync(Id, cancellationToken);
            return RedirectToPage("/Admin/Weapons/Index");
        }
    }
}