using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatHammer.Server.PageServices.Admin.Models
{
    public interface IModelAdminPageService
    {
        Task<List<ModelListItemViewModel>> GetModelsAsync(
            CancellationToken cancellationToken = default);

        Task<ModelListItemViewModel?> GetModelAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<ModelPageInput?> GetModelForEditAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<SelectListItem>> GetWeaponSelectListAsync(
            CancellationToken cancellationToken = default);

        Task<List<SelectListItem>> GetWargearSelectListAsync(
            CancellationToken cancellationToken = default);


        Task<int> CreateModelAsync(
            ModelPageInput input,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateModelAsync(
            int id,
            ModelPageInput input,
            CancellationToken cancellationToken = default);

        Task<ModelDeleteResult> DeleteModelAsync(
            int id,
            CancellationToken cancellationToken = default);
    }

    public class ModelDeleteResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }
    }
}