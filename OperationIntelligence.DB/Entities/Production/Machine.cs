namespace OperationIntelligence.DB;

public class Machine : AuditableEntity
{
    public string MachineCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public Guid WorkCenterId { get; set; }
    public WorkCenter WorkCenter { get; set; } = default!;

    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? SerialNumber { get; set; }

    public decimal HourlyRunningCost { get; set; }

    public MachineStatus Status { get; set; } = MachineStatus.Idle;

    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ProductionExecution> ProductionExecutions { get; set; } = new List<ProductionExecution>();
}