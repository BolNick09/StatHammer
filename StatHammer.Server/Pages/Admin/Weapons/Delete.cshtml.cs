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

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            Weapon = await _weaponService.GetWeaponAsync(id, cancellationToken);

            if (Weapon == null)
            {
                return NotFound();
            }

            Id = id;

            if (!Weapon.CanDelete)
            {
                ErrorMessage = BuildBlockedMessage(Weapon);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var result = await _weaponService.DeleteWeaponAsync(Id, cancellationToken);

            if (!result.Success)
            {
                Weapon = await _weaponService.GetWeaponAsync(Id, cancellationToken);
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            return RedirectToPage("/Admin/Weapons/Index");
        }

        private static string BuildBlockedMessage(WeaponListItemViewModel weapon)
        {
            var parts = new List<string>();

            if (weapon.IsUsedByModels)
            {
                parts.Add($"используется моделями: {string.Join(", ", weapon.UsedByModels)}");
            }

            if (weapon.IsUsedBySimulationResults)
            {
                parts.Add($"используется в сохранённых результатах: {weapon.SimulationResultUsageCount}");
            }

            return "Оружие не может быть удалено, потому что " + string.Join("; ", parts) + ".";
        }
    }
}