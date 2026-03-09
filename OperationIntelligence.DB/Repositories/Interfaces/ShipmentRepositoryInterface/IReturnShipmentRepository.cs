namespace OperationIntelligence.DB;

public interface IReturnShipmentRepository : IBaseRepository<ReturnShipment>
{
    Task<ReturnShipment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReturnShipment?> GetByReturnShipmentNumberAsync(string returnShipmentNumber, CancellationToken cancellationToken = default);
    Task<ReturnShipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReturnShipment>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        ReturnShipmentStatus? status = null,
        Guid? shipmentId = null,
        Guid? orderId = null,
        Guid? carrierId = null,
        DateTime? requestedFromUtc = null,
        DateTime? requestedToUtc = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? search = null,
        ReturnShipmentStatus? status = null,
        Guid? shipmentId = null,
        Guid? orderId = null,
        Guid? carrierId = null,
        DateTime? requestedFromUtc = null,
        DateTime? requestedToUtc = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReturnShipment>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReturnShipment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task<ReturnShipmentItem?> GetReturnItemByIdAsync(Guid returnShipmentItemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReturnShipmentItem>> GetItemsByReturnShipmentIdAsync(Guid returnShipmentId, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalReturnedQuantityForShipmentItemAsync(Guid shipmentItemId, CancellationToken cancellationToken = default);

    Task AddItemAsync(ReturnShipmentItem item, CancellationToken cancellationToken = default);
    void UpdateItem(ReturnShipmentItem item);
    void RemoveItem(ReturnShipmentItem item);
}