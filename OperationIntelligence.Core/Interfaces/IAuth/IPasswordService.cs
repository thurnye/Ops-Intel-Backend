namespace OperationIntelligence.Core
{
    public interface IPasswordService
    {
        string HashPassword(string rawPassword);
        bool VerifyPassword(string rawPassword, string passwordHash);
        bool IsStrongPassword(string password);
    }
}
