using System.ComponentModel.DataAnnotations;

namespace OperationIntelligence.DB;

public class MediaFile : AuditableEntity
{
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSizeInBytes { get; set; }

    [Required]
    [MaxLength(1000)]
    public string StoragePath { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? PublicUrl { get; set; }

    [MaxLength(128)]
    public string? ChecksumSha256 { get; set; }

    public bool IsPublic { get; set; } = false;
    public bool IsProcessed { get; set; } = false;
}
