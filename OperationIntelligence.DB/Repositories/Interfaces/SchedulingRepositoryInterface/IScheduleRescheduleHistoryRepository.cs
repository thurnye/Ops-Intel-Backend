namespace OperationIntelligence.DB;


public interface IScheduleRescheduleHistoryRepository
{
    Task<ScheduleRescheduleHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleRescheduleHistory>> GetBySchedulePlanIdAsync(Guid schedulePlanId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleRescheduleHistory>> GetByScheduleJobIdAsync(Guid scheduleJobId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleRescheduleHistory>> GetByScheduleOperationIdAsync(Guid scheduleOperationId, CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleRescheduleHistory entity, CancellationToken cancellationToken = default);
}
