using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Keywords;

namespace StatHammer.Server.Pages.Admin.Keywords
{
    public class DeleteModel : PageModel
    {
        private readonly IKeywordAdminPageService _keywordService;

        public DeleteModel(IKeywordAdminPageService keywordService)
        {
            _keywordService = keywordService;
        }

        [BindProperty]
        public int Id { get; set; }

        public KeywordListItemViewModel? Keyword { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
        {
            Keyword = await _keywordService.GetKeywordAsync(id, cancellationToken);

            if (Keyword == null)
            {
                return NotFound();
            }

            Id = id;

            if (!Keyword.CanDelete)
            {
                ErrorMessage =
                    $"Ключевое слово не может быть удалено, потому что используется юнитами: " +
                    $"{string.Join(", ", Keyword.UsedByUnits)}.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var result = await _keywordService.DeleteKeywordAsync(Id, cancellationToken);

            if (!result.Success)
            {
                Keyword = await _keywordService.GetKeywordAsync(Id, cancellationToken);
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            return RedirectToPage("/Admin/Keywords/Index");
        }
    }
}