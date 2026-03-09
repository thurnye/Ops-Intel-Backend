namespace OperationIntelligence.DB;

public class WorkCenter : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public decimal CapacityPerDay { get; set; }
    public int AvailableOperators { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Machine> Machines { get; set; } = new List<Machine>();
    public ICollection<RoutingStep> RoutingSteps { get; set; } = new List<RoutingStep>();
    public ICollection<ProductionExecution> ProductionExecutions { get; set; } = new List<ProductionExecution>();
}