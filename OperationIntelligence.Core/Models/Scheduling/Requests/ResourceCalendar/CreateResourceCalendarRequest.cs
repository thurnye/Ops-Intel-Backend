namespace OperationIntelligence.Core.Models.Scheduling.Requests.ResourceCalendar;

public class CreateResourceCalendarRequest
{
    public Guid ResourceId { get; set; }
    public int ResourceType { get; set; }

    public string CalendarName { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "UTC";

    public bool MondayEnabled { get; set; }
    public bool TuesdayEnabled { get; set; }
    public bool WednesdayEnabled { get; set; }
    public bool ThursdayEnabled { get; set; }
    public bool FridayEnabled { get; set; }
    public bool SaturdayEnabled { get; set; }
    public bool SundayEnabled { get; set; }

    public TimeSpan DefaultStartTime { get; set; }
    public TimeSpan DefaultEndTime { get; set; }

    public bool IsDefault { get; set; }
}
