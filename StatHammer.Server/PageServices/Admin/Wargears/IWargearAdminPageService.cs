namespace StatHammer.Server.PageServices.Admin.Wargears
{
    public interface IWargearAdminPageService
    {
        Task<List<WargearListItemViewModel>> GetWargearsAsync(
            CancellationToken cancellationToken = default);

        Task<WargearListItemViewModel?> GetWargearAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<int> CreateWargearAsync(
            WargearPageInput input,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateWargearAsync(
            int id,
            WargearPageInput input,
            CancellationToken cancellationToken = default);

        Task<WargearDeleteResult> DeleteWargearAsync(
            int id,
            CancellationToken cancellationToken = default);
    }

    public class WargearDeleteResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }
    }
}