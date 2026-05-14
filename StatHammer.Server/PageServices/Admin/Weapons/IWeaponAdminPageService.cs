namespace StatHammer.Server.PageServices.Admin.Weapons
{
    public interface IWeaponAdminPageService
    {
        Task<List<WeaponListItemViewModel>> GetWeaponsAsync(
            CancellationToken cancellationToken = default);

        Task<WeaponListItemViewModel?> GetWeaponAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<int> CreateWeaponAsync(
            CreateWeaponPageInput input,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteWeaponAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
