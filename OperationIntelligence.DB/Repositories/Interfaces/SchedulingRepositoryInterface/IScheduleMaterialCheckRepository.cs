namespace OperationIntelligence.DB;


public interface IScheduleMaterialCheckRepository
{
    Task<ScheduleMaterialCheck?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleMaterialCheck>> GetByScheduleJobIdAsync(Guid scheduleJobId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleMaterialCheck>> GetByScheduleOperationIdAsync(Guid scheduleOperationId, CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleMaterialCheck entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleMaterialCheck entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleMaterialCheck entity, CancellationToken cancellationToken = default);
}
