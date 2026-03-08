namespace OperationIntelligence.DB;

public class OrderImage : OrderBaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public string FileName { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public string FileExtension { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSizeBytes { get; set; }

    public string StoragePath { get; set; } = default!;
    public string? PublicUrl { get; set; }

    public OrderImageType ImageType { get; set; } = OrderImageType.General;
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }

    public DateTime UploadedAtUtc { get; set; }
    public string? UploadedBy { get; set; }

    public bool IsActive { get; set; } = true;
}
