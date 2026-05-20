using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatHammer.Server.PageServices.Admin.Units
{
    public interface IUnitAdminPageService
    {
        Task<List<UnitListItemViewModel>> GetUnitsAsync(
            CancellationToken cancellationToken = default);

        Task<UnitListItemViewModel?> GetUnitAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<UnitPageInput?> GetUnitForEditAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<SelectListItem>> GetModelSelectListAsync(
            CancellationToken cancellationToken = default);

        Task<List<SelectListItem>> GetKeywordSelectListAsync(
            CancellationToken cancellationToken = default);

        Task<int> CreateUnitAsync(
            UnitPageInput input,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateUnitAsync(
            int id,
            UnitPageInput input,
            CancellationToken cancellationToken = default);

        Task<UnitDeleteResult> DeleteUnitAsync(
            int id,
            CancellationToken cancellationToken = default);
    }

    public class UnitDeleteResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }
    }
}