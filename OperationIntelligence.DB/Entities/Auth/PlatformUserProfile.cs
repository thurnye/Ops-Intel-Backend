using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationIntelligence.DB;

public class PlatformUserProfile : AuditableEntity
{
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? DisplayName { get; set; }

    public DateTime? Birthdate { get; set; }

    [MaxLength(50)]
    public string? Gender { get; set; }

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [MaxLength(200)]
    public string? AddressLine1 { get; set; }

    [MaxLength(200)]
    public string? AddressLine2 { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? StateOrProvince { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    public Guid? AvatarFileId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual PlatformUser User { get; set; } = null!;

    [ForeignKey(nameof(AvatarFileId))]
    public virtual MediaFile? AvatarFile { get; set; }
}
