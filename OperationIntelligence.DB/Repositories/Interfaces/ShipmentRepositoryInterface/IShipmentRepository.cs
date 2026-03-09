namespace OperationIntelligence.DB;

public interface IShipmentRepository : IBaseRepository<Shipment>
{
    Task<Shipment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByShipmentNumberAsync(string shipmentNumber, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Shipment>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        ShipmentStatus? status = null,
        ShipmentType? type = null,
        ShipmentPriority? priority = null,
        Guid? orderId = null,
        Guid? warehouseId = null,
        Guid? carrierId = null,
        DateTime? plannedShipDateFromUtc = null,
        DateTime? plannedShipDateToUtc = null,
        bool? isCrossBorder = null,
        bool? isPartialShipment = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? search = null,
        ShipmentStatus? status = null,
        ShipmentType? type = null,
        ShipmentPriority? priority = null,
        Guid? orderId = null,
        Guid? warehouseId = null,
        Guid? carrierId = null,
        DateTime? plannedShipDateFromUtc = null,
        DateTime? plannedShipDateToUtc = null,
        bool? isCrossBorder = null,
        bool? isPartialShipment = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Shipment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetReadyToDispatchAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetDueForDeliveryAsync(DateTime asOfUtc, CancellationToken cancellationToken = default);

    Task<ShipmentItem?> GetShipmentItemByIdAsync(Guid shipmentItemId, CancellationToken cancellationToken = default);
    Task<ShipmentPackage?> GetShipmentPackageByIdAsync(Guid shipmentPackageId, CancellationToken cancellationToken = default);
    Task<ShipmentPackageItem?> GetShipmentPackageItemByIdAsync(Guid shipmentPackageItemId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentItem>> GetItemsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShipmentPackage>> GetPackagesByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShipmentTrackingEvent>> GetTrackingEventsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShipmentDocument>> GetDocumentsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShipmentCharge>> GetChargesByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShipmentStatusHistory>> GetStatusHistoryByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShipmentException>> GetExceptionsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShipmentInsurance>> GetInsurancesByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomsDocument>> GetCustomsDocumentsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalShippedQuantityForOrderItemAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalDeliveredQuantityForOrderItemAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalReturnedQuantityForShipmentItemAsync(Guid shipmentItemId, CancellationToken cancellationToken = default);

    Task AddItemAsync(ShipmentItem item, CancellationToken cancellationToken = default);
    void UpdateItem(ShipmentItem item);
    void RemoveItem(ShipmentItem item);

    Task AddPackageAsync(ShipmentPackage package, CancellationToken cancellationToken = default);
    void UpdatePackage(ShipmentPackage package);
    void RemovePackage(ShipmentPackage package);

    Task AddPackageItemAsync(ShipmentPackageItem packageItem, CancellationToken cancellationToken = default);
    void UpdatePackageItem(ShipmentPackageItem packageItem);
    void RemovePackageItem(ShipmentPackageItem packageItem);

    Task AddTrackingEventAsync(ShipmentTrackingEvent trackingEvent, CancellationToken cancellationToken = default);
    Task AddDocumentAsync(ShipmentDocument document, CancellationToken cancellationToken = default);
    Task AddChargeAsync(ShipmentCharge charge, CancellationToken cancellationToken = default);
    Task AddExceptionAsync(ShipmentException shipmentException, CancellationToken cancellationToken = default);
    Task AddInsuranceAsync(ShipmentInsurance insurance, CancellationToken cancellationToken = default);
    Task AddCustomsDocumentAsync(CustomsDocument customsDocument, CancellationToken cancellationToken = default);
    Task AddStatusHistoryAsync(ShipmentStatusHistory statusHistory, CancellationToken cancellationToken = default);
}