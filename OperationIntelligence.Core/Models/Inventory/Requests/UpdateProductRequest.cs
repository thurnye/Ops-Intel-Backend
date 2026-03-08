using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class UpdateProductRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }

    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid UnitOfMeasureId { get; set; }

    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal TaxRate { get; set; }

    public decimal ReorderLevel { get; set; }
    public decimal ReorderQuantity { get; set; }

    public bool TrackInventory { get; set; }
    public bool AllowBackOrder { get; set; }
    public bool IsSerialized { get; set; }
    public bool IsBatchTracked { get; set; }
    public bool IsPerishable { get; set; }

    public decimal Weight { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }

    public ProductStatus Status { get; set; }
    public string? ThumbnailImageUrl { get; set; }
}
