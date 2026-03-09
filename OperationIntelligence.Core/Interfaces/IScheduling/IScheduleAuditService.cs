using OperationIntelligence.Core.Models.Scheduling.Requests.Audit;
using OperationIntelligence.Core.Models.Scheduling.Responses.Audit;

namespace OperationIntelligence.Core;

public interface IScheduleAuditService
{
    Task<ScheduleAuditLogResponse> CreateAsync(
        CreateScheduleAuditLogRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleAuditLogResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<ScheduleAuditLogResponse>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
