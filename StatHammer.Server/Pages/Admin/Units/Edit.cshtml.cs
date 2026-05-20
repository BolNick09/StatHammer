using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatHammer.Server.PageServices.Admin.Units;

namespace StatHammer.Server.Pages.Admin.Units
{
    public class EditModel : PageModel
    {
        private readonly IUnitAdminPageService _unitService;

        public EditModel(IUnitAdminPageService unitService)
        {
            _unitService = unitService;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public UnitPageInput Input { get; set; } = new();

        public List<SelectListItem> ModelOptions { get; set; } = new();

        public List<SelectListItem> KeywordOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            await LoadSelectListsAsync(cancellationToken);

            var unit = await _unitService.GetUnitForEditAsync(id, cancellationToken);

            if (unit == null)
            {
                return NotFound();
            }

            Id = id;
            Input = unit;

            EnsureCompositionRows();

            return Page();
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
                var updated = await _unitService.UpdateUnitAsync(Id, Input, cancellationToken);

                if (!updated)
                {
                    return NotFound();
                }
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
    }
}