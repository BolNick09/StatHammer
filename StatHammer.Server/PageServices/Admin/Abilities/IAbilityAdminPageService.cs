namespace StatHammer.Server.PageServices.Admin.Abilities
{
    public interface IAbilityAdminPageService
    {
        Task<List<AbilityListItemViewModel>> GetAbilitiesAsync(
            CancellationToken cancellationToken = default);

        Task<AbilityListItemViewModel?> GetAbilityAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<int> CreateAbilityAsync(
            AbilityPageInput input,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAbilityAsync(
            int id,
            AbilityPageInput input,
            CancellationToken cancellationToken = default);

        Task<AbilityDeleteResult> DeleteAbilityAsync(
            int id,
            CancellationToken cancellationToken = default);
    }

    public class AbilityDeleteResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
