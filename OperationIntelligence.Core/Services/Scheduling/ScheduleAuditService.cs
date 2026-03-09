using OperationIntelligence.Core.Models.Scheduling.Requests.Audit;
using OperationIntelligence.Core.Models.Scheduling.Responses.Audit;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ScheduleAuditService : IScheduleAuditService
{
    private readonly IScheduleAuditLogRepository _scheduleAuditLogRepository;

    public ScheduleAuditService(IScheduleAuditLogRepository scheduleAuditLogRepository)
    {
        _scheduleAuditLogRepository = scheduleAuditLogRepository;
    }

    public async Task<ScheduleAuditLogResponse> CreateAsync(CreateScheduleAuditLogRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ScheduleAuditLog
        {
            EntityName = request.EntityName.Trim(),
            EntityId = request.EntityId,
            ActionType = request.ActionType.Trim(),
            ChangedFieldsJson = request.ChangedFieldsJson,
            OldValuesJson = request.OldValuesJson,
            NewValuesJson = request.NewValuesJson,
            Source = request.Source.Trim(),
            Reason = request.Reason?.Trim(),
            PerformedAtUtc = request.PerformedAtUtc,
            CorrelationId = request.CorrelationId?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _scheduleAuditLogRepository.AddAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleAuditLogResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleAuditLogRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<PagedResponse<ScheduleAuditLogResponse>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (items, totalRecords) = await _scheduleAuditLogRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);

        return new PagedResponse<ScheduleAuditLogResponse>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            Items = items.Select(MapToResponse).ToList()
        };
    }

    private static ScheduleAuditLogResponse MapToResponse(ScheduleAuditLog entity)
    {
        return new ScheduleAuditLogResponse
        {
            Id = entity.Id,
            EntityName = entity.EntityName,
            EntityId = entity.EntityId,
            ActionType = entity.ActionType,
            ChangedFieldsJson = entity.ChangedFieldsJson,
            OldValuesJson = entity.OldValuesJson,
            NewValuesJson = entity.NewValuesJson,
            Source = entity.Source,
            Reason = entity.Reason,
            PerformedAtUtc = entity.PerformedAtUtc,
            CorrelationId = entity.CorrelationId
        };
    }
}
