namespace OperationIntelligence.Core
{
    public interface INormalizationService
    {
        string NormalizeEmail(string email);
        string NormalizeUserName(string userName);
        string NormalizeRoleName(string roleName);
    }
}
