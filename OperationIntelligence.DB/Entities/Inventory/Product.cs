namespace OperationIntelligence.DB;

public class Product: AuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public string SKU { get; set; } = default!;
    public string? Barcode { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public Guid? BrandId { get; set; }
    public Brand? Brand { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;

    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }

    public decimal TaxRate { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal ReorderQuantity { get; set; }

    public bool TrackInventory { get; set; } = true;
    public bool AllowBackOrder { get; set; } = false;
    public bool IsSerialized { get; set; } = false;
    public bool IsBatchTracked { get; set; } = false;
    public bool IsPerishable { get; set; } = false;

    public decimal Weight { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }

    public ProductStatus Status { get; set; } = ProductStatus.Active;

    public string? ThumbnailImageUrl { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
}
