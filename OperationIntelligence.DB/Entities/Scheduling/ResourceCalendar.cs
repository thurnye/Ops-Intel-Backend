namespace OperationIntelligence.DB;

public class ResourceCalendar : AuditableEntity
{
    public Guid ResourceId { get; set; }
    public ResourceType ResourceType { get; set; }

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

    public ICollection<ResourceCalendarException> Exceptions { get; set; } = new List<ResourceCalendarException>();
}
