using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.PageServices.Admin.Keywords;

namespace StatHammer.Server.Pages.Admin.Keywords
{
    public class IndexModel : PageModel
    {
        private readonly IKeywordAdminPageService _keywordService;

        public IndexModel(IKeywordAdminPageService keywordService)
        {
            _keywordService = keywordService;
        }

        public List<KeywordListItemViewModel> Keywords { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Keywords = await _keywordService.GetKeywordsAsync(cancellationToken);
        }
    }
}