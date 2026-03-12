using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;
using OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleJob;

namespace OperationIntelligence.Core;

public interface IScheduleJobService
{
    Task<ScheduleJobResponse> CreateAsync(
        CreateScheduleJobRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleJobResponse> UpdateAsync(
        Guid id,
        UpdateScheduleJobRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleJobResponse> ReleaseAsync(
        Guid id,
        ReleaseScheduleJobRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleJobResponse> PauseAsync(
        Guid id,
        PauseScheduleJobRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleJobResponse> CancelAsync(
        Guid id,
        CancelScheduleJobRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleJobResponse> RescheduleAsync(
        Guid id,
        RescheduleJobRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleJobDetailResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<ScheduleJobResponse>> GetAllAsync(
        GetScheduleJobsRequest request,
        CancellationToken cancellationToken = default);

    Task<DispatchMetricsSummaryResponse> GetDispatchSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
