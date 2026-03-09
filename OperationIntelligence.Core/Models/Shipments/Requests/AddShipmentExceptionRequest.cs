using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class AddShipmentExceptionRequest
{
    public ShipmentExceptionType ExceptionType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReportedAtUtc { get; set; }
    public string? ReportedBy { get; set; }
}
