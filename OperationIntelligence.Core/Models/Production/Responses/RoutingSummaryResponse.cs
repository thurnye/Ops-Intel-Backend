namespace OperationIntelligence.Core.Models.Production.Responses;

public class RoutingSummaryResponse
{
    public Guid Id { get; set; }
    public string RoutingCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}
