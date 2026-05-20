using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatHammer.Server.PageServices.Admin.Units;

namespace StatHammer.Server.Pages.Admin.Units
{
    public class CreateModel : PageModel
    {
        private readonly IUnitAdminPageService _unitService;

        public CreateModel(IUnitAdminPageService unitService)
        {
            _unitService = unitService;
        }

        [BindProperty]
        public UnitPageInput Input { get; set; } = new();

        public List<SelectListItem> ModelOptions { get; set; } = new();

        public List<SelectListItem> KeywordOptions { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Input.Models = BuildEmptyCompositionRows();
            await LoadSelectListsAsync(cancellationToken);
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            await LoadSelectListsAsync(cancellationToken);

            if (!ModelState.IsValid)
            {
                EnsureCompositionRows();
                return Page();
            }

            try
            {
                await _unitService.CreateUnitAsync(Input, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                EnsureCompositionRows();
                return Page();
            }

            return RedirectToPage("/Admin/Units/Index");
        }

        private async Task LoadSelectListsAsync(CancellationToken cancellationToken)
        {
            ModelOptions = await _unitService.GetModelSelectListAsync(cancellationToken);
            KeywordOptions = await _unitService.GetKeywordSelectListAsync(cancellationToken);

            ViewData["ModelOptions"] = ModelOptions;
            ViewData["KeywordOptions"] = KeywordOptions;
        }

        private void EnsureCompositionRows()
        {
            while (Input.Models.Count < 8)
            {
                Input.Models.Add(new UnitModelCompositionInput());
            }
        }

        private static List<UnitModelCompositionInput> BuildEmptyCompositionRows()
        {
            return Enumerable.Range(0, 8)
                .Select(_ => new UnitModelCompositionInput())
                .ToList();
        }
    }
}