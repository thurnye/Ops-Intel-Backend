namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionLaborLogResponse
{
    public Guid Id { get; set; }
    public Guid ProductionExecutionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime WorkDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
