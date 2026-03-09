namespace OperationIntelligence.DB;

public interface IDeliveryRunRepository : IBaseRepository<DeliveryRun>
{
    Task<DeliveryRun?> GetByIdWithShipmentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DeliveryRun?> GetByRunNumberAsync(string runNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeliveryRun>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        DeliveryRunStatus? status = null,
        Guid? warehouseId = null,
        DateTime? plannedStartFromUtc = null,
        DateTime? plannedStartToUtc = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? search = null,
        DeliveryRunStatus? status = null,
        Guid? warehouseId = null,
        DateTime? plannedStartFromUtc = null,
        DateTime? plannedStartToUtc = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeliveryRun>> GetActiveRunsAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetAssignedShipmentsAsync(Guid deliveryRunId, CancellationToken cancellationToken = default);
    Task<int> CountAssignedShipmentsAsync(Guid deliveryRunId, CancellationToken cancellationToken = default);
}
