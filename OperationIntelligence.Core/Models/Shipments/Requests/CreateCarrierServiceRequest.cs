namespace OperationIntelligence.Core;

public class CreateCarrierServiceRequest
{
    public Guid CarrierId { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedTransitDays { get; set; }
    public bool IsActive { get; set; } = true;
}
