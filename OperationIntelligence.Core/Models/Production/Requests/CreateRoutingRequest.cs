namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateRoutingRequest
{
    public string RoutingCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Notes { get; set; }
}
