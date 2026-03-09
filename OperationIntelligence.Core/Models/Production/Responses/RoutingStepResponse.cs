namespace OperationIntelligence.Core.Models.Production.Responses;

public class RoutingStepResponse
{
    public Guid Id { get; set; }
    public Guid RoutingId { get; set; }
    public int Sequence { get; set; }
    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public Guid WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public decimal SetupTimeMinutes { get; set; }
    public decimal RunTimeMinutesPerUnit { get; set; }
    public decimal QueueTimeMinutes { get; set; }
    public decimal WaitTimeMinutes { get; set; }
    public decimal MoveTimeMinutes { get; set; }
    public int RequiredOperators { get; set; }
    public bool IsOutsourced { get; set; }
    public bool IsQualityCheckpointRequired { get; set; }
    public string? Instructions { get; set; }
    public string? Notes { get; set; }
}
