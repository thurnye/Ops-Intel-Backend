namespace OperationIntelligence.DB;

public class Routing : AuditableEntity
{
    public string RoutingCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public string? Notes { get; set; }

    public ICollection<RoutingStep> Steps { get; set; } = new List<RoutingStep>();
    public ICollection<ProductionOrder> ProductionOrders { get; set; } = new List<ProductionOrder>();
}