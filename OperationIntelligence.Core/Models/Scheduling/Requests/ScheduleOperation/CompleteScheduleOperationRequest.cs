namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class CompleteScheduleOperationRequest
{
    public DateTime ActualEndUtc { get; set; }
    public decimal CompletedQuantity { get; set; }
    public decimal ScrappedQuantity { get; set; }
    public Guid? ProductionExecutionId { get; set; }
    public string? Notes { get; set; }
}
