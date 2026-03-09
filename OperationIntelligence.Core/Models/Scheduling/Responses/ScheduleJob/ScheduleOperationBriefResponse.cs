namespace OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleJob;

public class ScheduleOperationBriefResponse
{
    public Guid Id { get; set; }
    public int SequenceNo { get; set; }
    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public DateTime PlannedStartUtc { get; set; }
    public DateTime PlannedEndUtc { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
}
