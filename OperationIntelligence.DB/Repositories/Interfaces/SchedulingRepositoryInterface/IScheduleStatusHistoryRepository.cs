namespace OperationIntelligence.DB;


public interface IScheduleStatusHistoryRepository
{
    Task<ScheduleStatusHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleStatusHistory>> GetBySchedulePlanIdAsync(Guid schedulePlanId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleStatusHistory>> GetByScheduleJobIdAsync(Guid scheduleJobId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleStatusHistory>> GetByScheduleOperationIdAsync(Guid scheduleOperationId, CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleStatusHistory entity, CancellationToken cancellationToken = default);
}
