namespace OperationIntelligence.Core;
    public class AuthUserContext
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public IReadOnlyList<string> Roles { get; set; } = new List<string>();
        public IReadOnlyList<string> Permissions { get; set; } = new List<string>();
        public bool IsAuthenticated { get; set; }
    }
