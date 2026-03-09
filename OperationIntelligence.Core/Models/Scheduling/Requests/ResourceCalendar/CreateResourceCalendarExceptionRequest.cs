namespace OperationIntelligence.Core.Models.Scheduling.Requests.ResourceCalendar;

public class CreateResourceCalendarExceptionRequest
{
    public Guid ResourceCalendarId { get; set; }
    public DateTime ExceptionStartUtc { get; set; }
    public DateTime ExceptionEndUtc { get; set; }
    public int ExceptionType { get; set; }
    public bool IsWorkingException { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
