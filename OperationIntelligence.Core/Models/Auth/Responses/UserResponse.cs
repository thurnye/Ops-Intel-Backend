namespace OperationIntelligence.Core
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }

        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }

        public DateTime? LastLoginAtUtc { get; set; }

        public UserProfileResponse Profile { get; set; } = new();
        public IReadOnlyList<string> Roles { get; set; } = new List<string>();
        public IReadOnlyList<string> Permissions { get; set; } = new List<string>();
    }
}
