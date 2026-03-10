using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationIntelligence.DB
{
    public class RefreshToken : AuditableEntity
    {
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(512)]
        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }

        [MaxLength(100)]
        public string? CreatedByIp { get; set; }

        [MaxLength(100)]
        public string? RevokedByIp { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        public Guid? ReplacedByTokenId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual PlatformUser User { get; set; } = null!;
    }
}
