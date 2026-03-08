namespace OperationIntelligence.DB;

public class OrderItem : OrderBaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public Guid? UnitOfMeasureId { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }

    public string ProductNameSnapshot { get; set; } = default!;
    public string ProductSkuSnapshot { get; set; } = default!;
    public string? ProductDescriptionSnapshot { get; set; }

    public decimal QuantityOrdered { get; set; }
    public decimal QuantityAllocated { get; set; }
    public decimal QuantityShipped { get; set; }
    public decimal QuantityDelivered { get; set; }
    public decimal QuantityCancelled { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }

    public string? Remarks { get; set; }
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}