using System.Net.Http.Json;
using StatHammer.DesktopClient.Models.Auth;

namespace StatHammer.DesktopClient.Services
{
    public class AuthService
    {
        private readonly ApiClient _apiClient;

        public AuthService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public LoginResponseDto? CurrentUser { get; private set; }

        public bool IsAuthenticated => CurrentUser != null;

        public async Task<LoginResponseDto> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken = default)
        {
            var request = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            var response = await _apiClient.HttpClient.PostAsJsonAsync(
                "api/Auth/login",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Не удалось выполнить вход. Проверьте email и пароль.");
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>(
                cancellationToken: cancellationToken);

            if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.Token))
            {
                throw new InvalidOperationException("Сервер вернул некорректный ответ авторизации.");
            }

            CurrentUser = loginResponse;
            _apiClient.SetBearerToken(loginResponse.Token);

            return loginResponse;
        }

        public void Logout()
        {
            CurrentUser = null;
            _apiClient.ClearBearerToken();
        }
    }
}