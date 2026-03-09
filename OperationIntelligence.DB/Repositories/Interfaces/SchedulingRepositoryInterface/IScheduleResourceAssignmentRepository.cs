namespace OperationIntelligence.DB;


public interface IScheduleResourceAssignmentRepository
{
    Task<ScheduleResourceAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleResourceAssignment>> GetByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleResourceAssignment entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleResourceAssignment entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleResourceAssignment entity, CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingAssignmentAsync(
        Guid resourceId,
        ResourceType resourceType,
        DateTime assignedStartUtc,
        DateTime assignedEndUtc,
        Guid? excludeAssignmentId = null,
        CancellationToken cancellationToken = default);
}
