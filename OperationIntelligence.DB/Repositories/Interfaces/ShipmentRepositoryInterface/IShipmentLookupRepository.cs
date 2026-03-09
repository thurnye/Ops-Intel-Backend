namespace OperationIntelligence.DB;

public interface IShipmentLookupRepository
{
    Task<bool> OrderExistsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<bool> OrderItemExistsAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<bool> WarehouseExistsAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<bool> UnitOfMeasureExistsAsync(Guid unitOfMeasureId, CancellationToken cancellationToken = default);
    Task<bool> InventoryStockExistsAsync(Guid inventoryStockId, CancellationToken cancellationToken = default);
    Task<bool> ProductionOrderExistsAsync(Guid productionOrderId, CancellationToken cancellationToken = default);
    Task<bool> ShipmentExistsAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<bool> ShipmentItemExistsAsync(Guid shipmentItemId, CancellationToken cancellationToken = default);
    Task<bool> CarrierExistsAsync(Guid carrierId, CancellationToken cancellationToken = default);
    Task<bool> CarrierServiceExistsAsync(Guid carrierServiceId, CancellationToken cancellationToken = default);
    Task<bool> ShipmentAddressExistsAsync(Guid shipmentAddressId, CancellationToken cancellationToken = default);
    Task<bool> DeliveryRunExistsAsync(Guid deliveryRunId, CancellationToken cancellationToken = default);
    Task<bool> DockAppointmentExistsAsync(Guid dockAppointmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Carrier>> GetActiveCarriersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierService>> GetActiveCarrierServicesAsync(Guid carrierId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Warehouse>> GetShipmentWarehousesAsync(CancellationToken cancellationToken = default);
}
