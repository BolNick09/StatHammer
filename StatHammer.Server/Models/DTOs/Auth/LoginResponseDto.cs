namespace StatHammer.Server.Models.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public string Email { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new();
    }
}