using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationIntelligence.DB;

public class PasswordResetToken : AuditableEntity
{
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(512)]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual PlatformUser User { get; set; } = null!;
}
