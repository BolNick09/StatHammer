using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatHammer.Server.PageServices.Admin.Weapons
{
    public interface IWeaponAdminPageService
    {
        Task<List<WeaponListItemViewModel>> GetWeaponsAsync(
            CancellationToken cancellationToken = default);

        Task<WeaponListItemViewModel?> GetWeaponAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<EditWeaponPageInput?> GetWeaponForEditAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<SelectListItem>> GetAbilitySelectListAsync(
            CancellationToken cancellationToken = default);

        Task<int> CreateWeaponAsync(
            CreateWeaponPageInput input,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateWeaponAsync(
            int id,
            EditWeaponPageInput input,
            CancellationToken cancellationToken = default);

        Task<WeaponDeleteResult> DeleteWeaponAsync(
            int id,
            CancellationToken cancellationToken = default);
    }

    public class WeaponDeleteResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }
    }
}