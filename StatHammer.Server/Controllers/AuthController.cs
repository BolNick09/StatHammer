using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StatHammer.Server.Models.DTOs.Auth;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(GetJwtExpiresMinutes());
            var token = CreateJwtToken(user, roles, expiresAtUtc);

            return Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            });
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<CurrentUserDto> Me()
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirstValue(ClaimTypes.Name)
                        ?? string.Empty;

            var roles = User.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new CurrentUserDto
            {
                Email = email,
                Roles = roles
            });
        }

        private string CreateJwtToken(
            ApplicationUser user,
            IEnumerable<string> roles,
            DateTime expiresAtUtc)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) ||
                string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("JWT settings are not configured.");
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int GetJwtExpiresMinutes()
        {
            var configuredValue = _configuration["Jwt:ExpiresMinutes"];

            if (int.TryParse(configuredValue, out var minutes) && minutes > 0)
            {
                return minutes;
            }

            return 120;
        }
    }
}