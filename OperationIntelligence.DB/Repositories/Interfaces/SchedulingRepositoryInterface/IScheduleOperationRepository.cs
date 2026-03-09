namespace OperationIntelligence.DB;


public interface IScheduleOperationRepository
{
    Task<ScheduleOperation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ScheduleOperation?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ScheduleOperation> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ScheduleOperation>> GetByScheduleJobIdAsync(Guid scheduleJobId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleOperation>> GetByWorkCenterAsync(Guid workCenterId, DateTime? startUtc = null, DateTime? endUtc = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleOperation>> GetByMachineAsync(Guid machineId, DateTime? startUtc = null, DateTime? endUtc = null, CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleOperation entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleOperation entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleOperation entity, CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingOperationOnWorkCenterAsync(Guid workCenterId, DateTime plannedStartUtc, DateTime plannedEndUtc, Guid? excludeOperationId = null, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingOperationOnMachineAsync(Guid machineId, DateTime plannedStartUtc, DateTime plannedEndUtc, Guid? excludeOperationId = null, CancellationToken cancellationToken = default);
}
