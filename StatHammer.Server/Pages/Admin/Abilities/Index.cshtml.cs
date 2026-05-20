using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Abilities;

namespace StatHammer.Server.Pages.Admin.Abilities
{
    public class IndexModel : PageModel
    {
        private readonly IAbilityAdminPageService _abilityService;

        public IndexModel(IAbilityAdminPageService abilityService)
        {
            _abilityService = abilityService;
        }

        public List<AbilityListItemViewModel> Abilities { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Abilities = await _abilityService.GetAbilitiesAsync(cancellationToken);
        }
    }
}