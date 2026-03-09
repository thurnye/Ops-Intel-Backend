using OperationIntelligence.Core.Models.Scheduling.Requests.Revision;
using OperationIntelligence.Core.Models.Scheduling.Responses.Revision;

namespace OperationIntelligence.Core;

public interface IScheduleRevisionService
{
    Task<ScheduleRevisionResponse> CreateRevisionAsync(
        CreateScheduleRevisionRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleRescheduleHistoryResponse> CreateRescheduleHistoryAsync(
        CreateScheduleRescheduleHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleStatusHistoryResponse> CreateStatusHistoryAsync(
        CreateScheduleStatusHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleRevisionResponse?> GetRevisionByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ScheduleRescheduleHistoryResponse?> GetRescheduleHistoryByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ScheduleStatusHistoryResponse?> GetStatusHistoryByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
