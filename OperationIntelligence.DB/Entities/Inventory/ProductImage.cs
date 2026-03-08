namespace OperationIntelligence.DB;

public class ProductImage: AuditableEntity

{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public string FileName { get; set; } = default!;
    public string FileUrl { get; set; } = default!;
    public string? ContentType { get; set; }
    public long FileSizeInBytes { get; set; }

    public bool IsPrimary { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;

    public string? AltText { get; set; }
}
