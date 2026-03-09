namespace OperationIntelligence.Core;

public class UpdateCarrierServiceRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedTransitDays { get; set; }
    public bool IsActive { get; set; }
}
