namespace OperationIntelligence.Core;

public class TokenGenerationResult
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
