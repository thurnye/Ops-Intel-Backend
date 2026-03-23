namespace OperationIntelligence.Core;

public class DashboardFilterRequest
{
    public DateRange Range { get; set; }
    public string Site { get; set; } = "all";
}

public class OverviewFilter
{
    public string Site { get; set; } = "all";
    public string Mode { get; set; } = "all";
    public string? Period { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
}

public class DateRange
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
}