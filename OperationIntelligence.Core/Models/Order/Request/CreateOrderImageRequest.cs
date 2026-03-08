namespace OperationIntelligence.Core;

public class CreateOrderImageRequest
{
    public Guid OrderId { get; set; }
    public string FileName { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public string FileExtension { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSizeBytes { get; set; }
    public string StoragePath { get; set; } = default!;
    public string? PublicUrl { get; set; }
    public OrderImageType ImageType { get; set; }
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }
    public string UploadedBy { get; set; } = default!;
}