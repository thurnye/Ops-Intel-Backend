using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentExceptionResponse
{
    public Guid Id { get; set; }
    public ShipmentExceptionType ExceptionType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReportedAtUtc { get; set; }
    public string? ReportedBy { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public string? ResolutionNote { get; set; }
}
