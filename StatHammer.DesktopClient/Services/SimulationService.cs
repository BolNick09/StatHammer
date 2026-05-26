using System.Net.Http.Json;
using StatHammer.DesktopClient.Models.Simulations;

namespace StatHammer.DesktopClient.Services
{
    public class SimulationService
    {
        private readonly ApiClient _apiClient;

        public SimulationService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<RunSimulationResponseDto> RunSimulationAsync(
            RunSimulationRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var response = await _apiClient.HttpClient.PostAsJsonAsync(
                "api/Simulations/run",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException(
                    $"Ошибка запуска симуляции: {(int)response.StatusCode} {response.ReasonPhrase}. {errorText}");
            }

            var result = await response.Content.ReadFromJsonAsync<RunSimulationResponseDto>(
                cancellationToken: cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Сервер вернул пустой результат симуляции.");
            }

            return result;
        }
    }
}