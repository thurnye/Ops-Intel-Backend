using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductListItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }

    public string CategoryName { get; set; } = string.Empty;
    public string? BrandName { get; set; }

    public decimal SellingPrice { get; set; }
    public ProductStatus Status { get; set; }

    public string? ThumbnailImageUrl { get; set; }
}
