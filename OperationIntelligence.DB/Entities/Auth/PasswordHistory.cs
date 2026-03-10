using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationIntelligence.DB;

public class PasswordHistory : AuditableEntity
{
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public virtual PlatformUser User { get; set; } = null!;
}
