namespace OperationIntelligence.Core;
public class ProductImageResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long FileSizeInBytes { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public string? AltText { get; set; }
}
