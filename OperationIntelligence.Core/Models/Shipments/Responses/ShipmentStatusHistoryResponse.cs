using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentStatusHistoryResponse
{
    public Guid Id { get; set; }
    public ShipmentStatus FromStatus { get; set; }
    public ShipmentStatus ToStatus { get; set; }
    public DateTime ChangedAtUtc { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
