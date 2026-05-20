namespace StatHammer.Server.PageServices.Admin.Keywords
{
    public interface IKeywordAdminPageService
    {
        Task<List<KeywordListItemViewModel>> GetKeywordsAsync(
            CancellationToken cancellationToken = default);

        Task<KeywordListItemViewModel?> GetKeywordAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<int> CreateKeywordAsync(
            KeywordPageInput input,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateKeywordAsync(
            int id,
            KeywordPageInput input,
            CancellationToken cancellationToken = default);

        Task<KeywordDeleteResult> DeleteKeywordAsync(
            int id,
            CancellationToken cancellationToken = default);
    }

    public class KeywordDeleteResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }
    }
}