namespace OperationIntelligence.DB;


public interface IScheduleOperationConstraintRepository
{
    Task<ScheduleOperationConstraint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleOperationConstraint>> GetByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleOperationConstraint entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleOperationConstraint entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleOperationConstraint entity, CancellationToken cancellationToken = default);
}
