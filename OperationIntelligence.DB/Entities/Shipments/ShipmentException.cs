namespace OperationIntelligence.DB;

public class ShipmentException : AuditableEntity
{
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;

    public ShipmentExceptionType ExceptionType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime ReportedAtUtc { get; set; }
    public string? ReportedBy { get; set; }

    public bool IsResolved { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public string? ResolutionNote { get; set; }
}