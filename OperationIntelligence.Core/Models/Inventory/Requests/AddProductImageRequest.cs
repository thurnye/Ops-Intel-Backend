namespace OperationIntelligence.Core;

public class AddProductImageRequest
{
    public Guid ProductId { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long FileSizeInBytes { get; set; }

    public bool IsPrimary { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public string? AltText { get; set; }
}
