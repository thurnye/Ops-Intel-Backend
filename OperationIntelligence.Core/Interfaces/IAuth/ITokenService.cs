using OperationIntelligence.DB;

namespace OperationIntelligence.Core
{
    public interface ITokenService
    {
        string GenerateAccessToken(
            PlatformUser user,
            IEnumerable<string> roles,
            IEnumerable<string> permissions);

        DateTime GetAccessTokenExpiryUtc();

        string GenerateSecureRefreshToken();
        string GenerateSecureEmailVerificationToken();
        string GenerateSecurePasswordResetToken();

        string HashToken(string rawToken);
    }
}
