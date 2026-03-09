namespace OperationIntelligence.Core.Models.Scheduling.Requests.Exception;

public class UpdateScheduleExceptionStatusRequest
{
    public int Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? ResolutionNotes { get; set; }
}
