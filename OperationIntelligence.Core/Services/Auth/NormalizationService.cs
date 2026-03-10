
namespace OperationIntelligence.Core
{
    public class NormalizationService : INormalizationService
    {
        public string NormalizeEmail(string email)
        {
            return string.IsNullOrWhiteSpace(email)
                ? string.Empty
                : email.Trim().ToUpperInvariant();
        }

        public string NormalizeUserName(string userName)
        {
            return string.IsNullOrWhiteSpace(userName)
                ? string.Empty
                : userName.Trim().ToUpperInvariant();
        }

        public string NormalizeRoleName(string roleName)
        {
            return string.IsNullOrWhiteSpace(roleName)
                ? string.Empty
                : roleName.Trim().ToUpperInvariant();
        }
    }
}
