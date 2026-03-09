using OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;
using OperationIntelligence.Core.Models.Scheduling.Responses.SchedulePlan;

namespace OperationIntelligence.Core;

public interface ISchedulePlanService
{
    Task<SchedulePlanResponse> CreateAsync(
        CreateSchedulePlanRequest request,
        CancellationToken cancellationToken = default);

    Task<SchedulePlanResponse> UpdateAsync(
        Guid id,
        UpdateSchedulePlanRequest request,
        CancellationToken cancellationToken = default);

    Task<SchedulePlanResponse> PublishAsync(
        Guid id,
        PublishSchedulePlanRequest request,
        CancellationToken cancellationToken = default);

    Task<SchedulePlanResponse> CloneAsync(
        Guid id,
        CloneSchedulePlanRequest request,
        CancellationToken cancellationToken = default);

    Task<SchedulePlanDetailResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<SchedulePlanResponse>> GetAllAsync(
        GetSchedulePlansRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
