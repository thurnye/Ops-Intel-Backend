using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateProductionDowntimeRequest
{
    public Guid ProductionExecutionId { get; set; }
    public DowntimeReasonType Reason { get; set; }
    public string? ReasonDescription { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsPlanned { get; set; }
    public string? Notes { get; set; }
}
