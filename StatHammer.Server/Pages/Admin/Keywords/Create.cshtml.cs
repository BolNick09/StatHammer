using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Keywords;

namespace StatHammer.Server.Pages.Admin.Keywords
{
    public class CreateModel : PageModel
    {
        private readonly IKeywordAdminPageService _keywordService;

        public CreateModel(IKeywordAdminPageService keywordService)
        {
            _keywordService = keywordService;
        }

        [BindProperty]
        public KeywordPageInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _keywordService.CreateKeywordAsync(Input, cancellationToken);
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