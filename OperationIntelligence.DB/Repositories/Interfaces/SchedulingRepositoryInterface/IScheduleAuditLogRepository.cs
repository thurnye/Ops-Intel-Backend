namespace OperationIntelligence.DB;

public interface IScheduleAuditLogRepository
{
    Task<ScheduleAuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ScheduleAuditLog> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleAuditLog entity, CancellationToken cancellationToken = default);
}
