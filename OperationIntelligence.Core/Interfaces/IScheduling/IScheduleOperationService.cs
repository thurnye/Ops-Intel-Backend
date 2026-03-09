using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleOperation;

namespace OperationIntelligence.Core;

public interface IScheduleOperationService
{
    Task<ScheduleOperationResponse> CreateAsync(
        CreateScheduleOperationRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationResponse> UpdateAsync(
        Guid id,
        UpdateScheduleOperationRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationResponse> StartAsync(
        Guid id,
        StartScheduleOperationRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationResponse> PauseAsync(
        Guid id,
        PauseScheduleOperationRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationResponse> CompleteAsync(
        Guid id,
        CompleteScheduleOperationRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationResponse> RescheduleAsync(
        Guid id,
        RescheduleOperationRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationDependencyResponse> AddDependencyAsync(
        CreateScheduleOperationDependencyRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveDependencyAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationConstraintResponse> AddConstraintAsync(
        CreateScheduleOperationConstraintRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationConstraintResponse> ResolveConstraintAsync(
        Guid id,
        ResolveScheduleOperationConstraintRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveConstraintAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationResourceOptionResponse> AddResourceOptionAsync(
        CreateScheduleOperationResourceOptionRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveResourceOptionAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ScheduleResourceAssignmentResponse> AddResourceAssignmentAsync(
        CreateScheduleResourceAssignmentRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleResourceAssignmentResponse> UpdateResourceAssignmentAsync(
        Guid id,
        UpdateScheduleResourceAssignmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveResourceAssignmentAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ScheduleOperationDetailResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<ScheduleOperationResponse>> GetAllAsync(
        GetScheduleOperationsRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
