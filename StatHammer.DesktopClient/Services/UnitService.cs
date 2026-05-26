using System.Net.Http.Json;
using StatHammer.DesktopClient.Models.Units;

namespace StatHammer.DesktopClient.Services
{
    public class UnitService
    {
        private readonly ApiClient _apiClient;

        public UnitService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<UnitListItemDto>> GetUnitsAsync(
            CancellationToken cancellationToken = default)
        {
            var units = await _apiClient.HttpClient.GetFromJsonAsync<List<UnitListItemDto>>(
                "api/Units",
                cancellationToken);

            return units ?? new List<UnitListItemDto>();
        }
    }
}