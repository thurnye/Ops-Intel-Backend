using OperationIntelligence.DB.Entities;

namespace OperationIntelligence.DB;

public class ProductionLaborLog : AuditableEntity
{
    public Guid ProductionExecutionId { get; set; }
    public ProductionExecution ProductionExecution { get; set; } = default!;

    public string UserId { get; set; } = string.Empty;
    public PlatformUser User { get; set; } = default!;

    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }

    public DateTime WorkDate { get; set; }

    public string? Notes { get; set; }
}
