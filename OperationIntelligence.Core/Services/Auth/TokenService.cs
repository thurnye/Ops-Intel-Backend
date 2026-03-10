using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(
            PlatformUser user,
            IEnumerable<string> roles,
            IEnumerable<string> permissions)
        {
            var jwt = _configuration.GetSection("JwtSettings");
            var key = jwt["Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
            var issuer = jwt["Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
            var audience = jwt["Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");

            if (!double.TryParse(jwt["DurationInMinutes"], out var durationMinutes))
            {
                durationMinutes = 30;
            }

            var expires = DateTime.UtcNow.AddMinutes(durationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                claims.Add(new Claim("username", user.UserName));
            }

            claims.AddRange(
                roles.Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(role => new Claim(ClaimTypes.Role, role)));

            claims.AddRange(
                permissions.Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(permission => new Claim("permission", permission)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetAccessTokenExpiryUtc()
        {
            var jwt = _configuration.GetSection("JwtSettings");

            if (!double.TryParse(jwt["DurationInMinutes"], out var durationMinutes))
            {
                durationMinutes = 30;
            }

            return DateTime.UtcNow.AddMinutes(durationMinutes);
        }

        public string GenerateSecureRefreshToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        public string GenerateSecureEmailVerificationToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));

        public string GenerateSecurePasswordResetToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));

        public string HashToken(string rawToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToBase64String(bytes);
        }
    }
}
