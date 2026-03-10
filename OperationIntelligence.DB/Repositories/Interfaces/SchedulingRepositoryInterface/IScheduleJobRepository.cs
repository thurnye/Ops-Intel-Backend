namespace OperationIntelligence.DB;


public interface IScheduleJobRepository
{
    Task<ScheduleJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ScheduleJob?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ScheduleJob?> GetByJobNumberAsync(string jobNumber, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ScheduleJob> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        DateTime? startDateUtc = null,
        DateTime? endDateUtc = null,
        Guid? schedulePlanId = null,
        Guid? productionOrderId = null,
        Guid? orderId = null,
        Guid? productId = null,
        Guid? warehouseId = null,
        int? status = null,
        int? priority = null,
        bool? materialsReady = null,
        int? materialReadinessStatus = null,
        bool? qualityHold = null,
        bool? isRushOrder = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleJob entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleJob entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleJob entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsByJobNumberAsync(string jobNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsForProductionOrderAsync(Guid productionOrderId, CancellationToken cancellationToken = default);
}
