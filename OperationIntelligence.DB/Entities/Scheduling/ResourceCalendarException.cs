namespace OperationIntelligence.DB;

public class ResourceCalendarException : AuditableEntity
{
    public Guid ResourceCalendarId { get; set; }
    public ResourceCalendar ResourceCalendar { get; set; } = default!;

    public DateTime ExceptionStartUtc { get; set; }
    public DateTime ExceptionEndUtc { get; set; }

    public CalendarExceptionType ExceptionType { get; set; }
    public bool IsWorkingException { get; set; }

    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
