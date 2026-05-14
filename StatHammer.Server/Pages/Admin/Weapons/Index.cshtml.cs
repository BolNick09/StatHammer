using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Weapons;

namespace StatHammer.Server.Pages.Admin.Weapons
{
    public class IndexModel : PageModel
    {
        private readonly IWeaponAdminPageService _weaponService;

        public IndexModel(IWeaponAdminPageService weaponService)
        {
            _weaponService = weaponService;
        }

        public List<WeaponListItemViewModel> Weapons { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Weapons = await _weaponService.GetWeaponsAsync(cancellationToken);
        }
    }

}
