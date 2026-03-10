using System.ComponentModel.DataAnnotations;

namespace OperationIntelligence.DB;

public class LoginAttempt : AuditableEntity
{
    public Guid? UserId { get; set; }

    [MaxLength(256)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public bool IsSuccessful { get; set; }

    [MaxLength(200)]
    public string? FailureReason { get; set; }

    public virtual PlatformUser? User { get; set; }
}
