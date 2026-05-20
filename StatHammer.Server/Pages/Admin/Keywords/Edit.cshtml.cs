using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Keywords;

namespace StatHammer.Server.Pages.Admin.Keywords
{
    public class EditModel : PageModel
    {
        private readonly IKeywordAdminPageService _keywordService;

        public EditModel(IKeywordAdminPageService keywordService)
        {
            _keywordService = keywordService;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public KeywordPageInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            var keyword = await _keywordService.GetKeywordAsync(id, cancellationToken);

            if (keyword == null)
            {
                return NotFound();
            }

            Id = keyword.Id;
            Input = new KeywordPageInput
            {
                Name = keyword.Name
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var updated = await _keywordService.UpdateKeywordAsync(Id, Input, cancellationToken);

                if (!updated)
                {
                    return NotFound();
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("/Admin/Keywords/Index");
        }
    }
}