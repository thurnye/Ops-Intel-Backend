using OperationIntelligence.Core.Models.Scheduling.Responses.Material;

namespace OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleJob;

public class ScheduleJobDetailResponse : ScheduleJobResponse
{
    public int TotalOperations { get; set; }
    public int TotalExceptions { get; set; }
    public int TotalMaterialChecks { get; set; }

    public List<ScheduleOperationBriefResponse> Operations { get; set; } = new();
    public List<ScheduleMaterialCheckResponse> MaterialChecks { get; set; } = new();
}
