using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ShipmentLookupRepository : IShipmentLookupRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ShipmentLookupRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public Task<bool> OrderExistsAsync(Guid orderId, CancellationToken cancellationToken = default)
        => _context.Orders.AnyAsync(x => x.Id == orderId, cancellationToken);

    public Task<bool> OrderItemExistsAsync(Guid orderItemId, CancellationToken cancellationToken = default)
        => _context.OrderItems.AnyAsync(x => x.Id == orderItemId, cancellationToken);

    public Task<bool> WarehouseExistsAsync(Guid warehouseId, CancellationToken cancellationToken = default)
        => _context.Warehouses.AnyAsync(x => x.Id == warehouseId, cancellationToken);

    public Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken = default)
        => _context.Products.AnyAsync(x => x.Id == productId, cancellationToken);

    public Task<bool> UnitOfMeasureExistsAsync(Guid unitOfMeasureId, CancellationToken cancellationToken = default)
        => _context.UnitsOfMeasure.AnyAsync(x => x.Id == unitOfMeasureId, cancellationToken);

    public Task<bool> InventoryStockExistsAsync(Guid inventoryStockId, CancellationToken cancellationToken = default)
        => _context.InventoryStocks.AnyAsync(x => x.Id == inventoryStockId, cancellationToken);

    public Task<bool> ProductionOrderExistsAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
        => _context.ProductionOrders.AnyAsync(x => x.Id == productionOrderId, cancellationToken);

    public Task<bool> ShipmentExistsAsync(Guid shipmentId, CancellationToken cancellationToken = default)
        => _context.Shipments.AnyAsync(x => x.Id == shipmentId, cancellationToken);

    public Task<bool> ShipmentItemExistsAsync(Guid shipmentItemId, CancellationToken cancellationToken = default)
        => _context.ShipmentItems.AnyAsync(x => x.Id == shipmentItemId, cancellationToken);

    public Task<bool> CarrierExistsAsync(Guid carrierId, CancellationToken cancellationToken = default)
        => _context.Carriers.AnyAsync(x => x.Id == carrierId, cancellationToken);

    public Task<bool> CarrierServiceExistsAsync(Guid carrierServiceId, CancellationToken cancellationToken = default)
        => _context.CarrierServices.AnyAsync(x => x.Id == carrierServiceId, cancellationToken);

    public Task<bool> ShipmentAddressExistsAsync(Guid shipmentAddressId, CancellationToken cancellationToken = default)
        => _context.ShipmentAddresses.AnyAsync(x => x.Id == shipmentAddressId, cancellationToken);

    public Task<bool> DeliveryRunExistsAsync(Guid deliveryRunId, CancellationToken cancellationToken = default)
        => _context.DeliveryRuns.AnyAsync(x => x.Id == deliveryRunId, cancellationToken);

    public Task<bool> DockAppointmentExistsAsync(Guid dockAppointmentId, CancellationToken cancellationToken = default)
        => _context.DockAppointments.AnyAsync(x => x.Id == dockAppointmentId, cancellationToken);

    public async Task<IReadOnlyList<Carrier>> GetActiveCarriersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Carriers
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CarrierService>> GetActiveCarrierServicesAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        return await _context.CarrierServices
            .AsNoTracking()
            .Where(x => x.CarrierId == carrierId && x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Warehouse>> GetShipmentWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
