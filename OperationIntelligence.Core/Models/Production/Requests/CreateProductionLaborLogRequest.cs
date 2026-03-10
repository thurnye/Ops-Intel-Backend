namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateProductionLaborLogRequest
{
    public Guid ProductionExecutionId { get; set; }
    public Guid UserId { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime WorkDate { get; set; }
    public string? Notes { get; set; }
}
