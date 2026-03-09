namespace OperationIntelligence.DB;


public interface IScheduleJobRepository
{
    Task<ScheduleJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ScheduleJob?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ScheduleJob?> GetByJobNumberAsync(string jobNumber, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ScheduleJob> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleJob entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleJob entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleJob entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsByJobNumberAsync(string jobNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsForProductionOrderAsync(Guid productionOrderId, CancellationToken cancellationToken = default);
}
