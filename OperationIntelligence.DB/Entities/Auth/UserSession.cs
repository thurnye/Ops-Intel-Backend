using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationIntelligence.DB;

public class UserSession : AuditableEntity
{
    public Guid UserId { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [MaxLength(100)]
    public string? DeviceName { get; set; }

    [MaxLength(100)]
    public string? Browser { get; set; }

    [MaxLength(100)]
    public string? OperatingSystem { get; set; }

    public DateTime LastSeenAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual PlatformUser User { get; set; } = null!;
}
