namespace StatHammer.Server.PageServices.Admin.SimulationResults
{
    public interface ISimulationResultAdminPageService
    {
        Task<List<SimulationResultListItemViewModel>> GetResultsAsync(
            CancellationToken cancellationToken = default);

        Task<SimulationResultDetailsViewModel?> GetResultDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteResultAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}