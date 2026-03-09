namespace OperationIntelligence.Core.Models.Scheduling.Responses.ResourceCalendar;

public class ResourceCalendarExceptionResponse
{
    public Guid Id { get; set; }
    public Guid ResourceCalendarId { get; set; }
    public DateTime ExceptionStartUtc { get; set; }
    public DateTime ExceptionEndUtc { get; set; }
    public int ExceptionType { get; set; }
    public string ExceptionTypeName { get; set; } = string.Empty;
    public bool IsWorkingException { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
