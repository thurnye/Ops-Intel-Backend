using System.ComponentModel.DataAnnotations;

namespace OperationIntelligence.DB;

public class PlatformUser : AuditableEntity
{
    [Required]
    [MaxLength(256)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [MaxLength(256)]
    public string LastName { get; set; } = string.Empty;
    [Required]

    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;


    [MaxLength(256)]
    public string Avatar { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string NormalizedEmail { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? UserName { get; set; }

    [MaxLength(100)]
    public string? NormalizedUserName { get; set; }

    [Required]
    [MaxLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? PasswordSalt { get; set; }

    public bool EmailConfirmed { get; set; } = false;
    public bool PhoneNumberConfirmed { get; set; } = false;
    public bool TwoFactorEnabled { get; set; } = false;

    public bool IsActive { get; set; } = true;
    public bool IsLocked { get; set; } = false;

    public int AccessFailedCount { get; set; } = 0;
    public DateTime? LockoutEndUtc { get; set; }

    public DateTime? LastLoginAtUtc { get; set; }
    public DateTime? PasswordChangedAtUtc { get; set; }

    [MaxLength(50)]
    public string AuthProvider { get; set; } = "Local"; // Local, Google, Microsoft, etc.

    [MaxLength(200)]
    public string? ExternalProviderId { get; set; }

    public virtual PlatformUserProfile? Profile { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    public virtual ICollection<PasswordHistory> PasswordHistories { get; set; } = new List<PasswordHistory>();
}
