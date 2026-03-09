using OperationIntelligence.Core.Models.Scheduling.Requests.Exception;
using OperationIntelligence.Core.Models.Scheduling.Responses.Exception;

namespace OperationIntelligence.Core;

public interface IScheduleExceptionService
{
    Task<ScheduleExceptionResponse> CreateAsync(
        CreateScheduleExceptionRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleExceptionResponse> ResolveAsync(
        Guid id,
        ResolveScheduleExceptionRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleExceptionResponse> UpdateStatusAsync(
        Guid id,
        UpdateScheduleExceptionStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleExceptionResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<ScheduleExceptionResponse>> GetAllAsync(
        GetScheduleExceptionsRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
