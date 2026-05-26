using System.Net.Http;
using System.Net.Http.Headers;

namespace StatHammer.DesktopClient.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7009/")
            };
        }

        public HttpClient HttpClient => _httpClient;

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearBearerToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}