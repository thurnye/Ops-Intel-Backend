using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ShipmentRepository : BaseRepository<Shipment>, IShipmentRepository
{
    public ShipmentRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Shipment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Order)
            .Include(x => x.Warehouse)
            .Include(x => x.Carrier)
            .Include(x => x.CarrierService)
            .Include(x => x.OriginAddress)
            .Include(x => x.DestinationAddress)
            .Include(x => x.DeliveryRun)
            .Include(x => x.DockAppointment)
            .Include(x => x.Items)
                .ThenInclude(x => x.OrderItem)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .Include(x => x.Items)
                .ThenInclude(x => x.UnitOfMeasure)
            .Include(x => x.Items)
                .ThenInclude(x => x.InventoryStock)
            .Include(x => x.Items)
                .ThenInclude(x => x.ProductionOrder)
            .Include(x => x.Packages)
                .ThenInclude(x => x.PackageItems)
            .Include(x => x.TrackingEvents)
            .Include(x => x.Documents)
            .Include(x => x.StatusHistories)
            .Include(x => x.Exceptions)
            .Include(x => x.Charges)
            .Include(x => x.Insurances)
            .Include(x => x.CustomsDocuments)
            .Include(x => x.ReturnShipments)
                .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Shipment?> GetByShipmentNumberAsync(string shipmentNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ShipmentNumber == shipmentNumber, cancellationToken);
    }

    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TrackingNumber == trackingNumber ||
                x.MasterTrackingNumber == trackingNumber,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = BuildShipmentFilterQuery(
            _dbSet.AsNoTracking()
                .Include(x => x.Order)
                .Include(x => x.Warehouse)
                .Include(x => x.Carrier)
                .Include(x => x.CarrierService),
            search,
            status,
            type,
            priority,
            orderId,
            warehouseId,
            carrierId,
            plannedShipDateFromUtc,
            plannedShipDateToUtc,
            isCrossBorder,
            isPartialShipment);

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = BuildShipmentFilterQuery(
            _dbSet.AsNoTracking(),
            search,
            status,
            type,
            priority,
            orderId,
            warehouseId,
            carrierId,
            plannedShipDateFromUtc,
            plannedShipDateToUtc,
            isCrossBorder,
            isPartialShipment);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.WarehouseId == warehouseId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetReadyToDispatchAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var statuses = new[]
        {
            ShipmentStatus.Packed,
            ShipmentStatus.ReadyToDispatch
        };

        var query = _dbSet
            .AsNoTracking()
            .Include(x => x.Order)
            .Include(x => x.Warehouse)
            .Include(x => x.Carrier)
            .Where(x => statuses.Contains(x.Status));

        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        return await query
            .OrderBy(x => x.PlannedShipDateUtc)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetDueForDeliveryAsync(DateTime asOfUtc, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Order)
            .Include(x => x.Warehouse)
            .Include(x => x.Carrier)
            .Where(x =>
                x.PlannedDeliveryDateUtc.HasValue &&
                x.PlannedDeliveryDateUtc.Value <= asOfUtc &&
                x.Status != ShipmentStatus.Delivered &&
                x.Status != ShipmentStatus.Returned &&
                x.Status != ShipmentStatus.Cancelled)
            .OrderBy(x => x.PlannedDeliveryDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShipmentItem?> GetShipmentItemByIdAsync(Guid shipmentItemId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentItems
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.OrderItem)
            .Include(x => x.UnitOfMeasure)
            .FirstOrDefaultAsync(x => x.Id == shipmentItemId, cancellationToken);
    }

    public async Task<ShipmentPackage?> GetShipmentPackageByIdAsync(Guid shipmentPackageId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentPackages
            .AsNoTracking()
            .Include(x => x.PackageItems)
            .FirstOrDefaultAsync(x => x.Id == shipmentPackageId, cancellationToken);
    }

    public async Task<ShipmentPackageItem?> GetShipmentPackageItemByIdAsync(Guid shipmentPackageItemId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentPackageItems
            .AsNoTracking()
            .Include(x => x.ShipmentPackage)
            .Include(x => x.ShipmentItem)
            .FirstOrDefaultAsync(x => x.Id == shipmentPackageItemId, cancellationToken);
    }

    public async Task<IReadOnlyList<ShipmentItem>> GetItemsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentItems
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.OrderItem)
            .Include(x => x.UnitOfMeasure)
            .Where(x => x.ShipmentId == shipmentId)
            .OrderBy(x => x.LineNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShipmentPackage>> GetPackagesByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentPackages
            .AsNoTracking()
            .Include(x => x.PackageItems)
            .Where(x => x.ShipmentId == shipmentId)
            .OrderBy(x => x.PackageNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShipmentTrackingEvent>> GetTrackingEventsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentTrackingEvents
            .AsNoTracking()
            .Where(x => x.ShipmentId == shipmentId)
            .OrderByDescending(x => x.EventTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShipmentDocument>> GetDocumentsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentDocuments
            .AsNoTracking()
            .Where(x => x.ShipmentId == shipmentId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShipmentCharge>> GetChargesByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentCharges
            .AsNoTracking()
            .Where(x => x.ShipmentId == shipmentId)
            .OrderBy(x => x.ChargeType)
            .ThenBy(x => x.Description)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShipmentStatusHistory>> GetStatusHistoryByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentStatusHistories
            .AsNoTracking()
            .Where(x => x.ShipmentId == shipmentId)
            .OrderByDescending(x => x.ChangedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShipmentException>> GetExceptionsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentExceptions
            .AsNoTracking()
            .Where(x => x.ShipmentId == shipmentId)
            .OrderByDescending(x => x.ReportedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShipmentInsurance>> GetInsurancesByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentInsurances
            .AsNoTracking()
            .Where(x => x.ShipmentId == shipmentId)
            .OrderByDescending(x => x.EffectiveDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CustomsDocument>> GetCustomsDocumentsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomsDocuments
            .AsNoTracking()
            .Where(x => x.ShipmentId == shipmentId)
            .OrderByDescending(x => x.IssuedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalShippedQuantityForOrderItemAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentItems
            .AsNoTracking()
            .Where(x => x.OrderItemId == orderItemId)
            .SumAsync(x => (decimal?)x.ShippedQuantity, cancellationToken) ?? 0m;
    }

    public async Task<decimal> GetTotalDeliveredQuantityForOrderItemAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return await _context.ShipmentItems
            .AsNoTracking()
            .Where(x => x.OrderItemId == orderItemId)
            .SumAsync(x => (decimal?)x.DeliveredQuantity, cancellationToken) ?? 0m;
    }

    public async Task<decimal> GetTotalReturnedQuantityForShipmentItemAsync(Guid shipmentItemId, CancellationToken cancellationToken = default)
    {
        return await _context.ReturnShipmentItems
            .AsNoTracking()
            .Where(x => x.ShipmentItemId == shipmentItemId)
            .SumAsync(x => (decimal?)x.ReturnedQuantity, cancellationToken) ?? 0m;
    }

    private static IQueryable<Shipment> BuildShipmentFilterQuery(
        IQueryable<Shipment> query,
        string? search,
        ShipmentStatus? status,
        ShipmentType? type,
        ShipmentPriority? priority,
        Guid? orderId,
        Guid? warehouseId,
        Guid? carrierId,
        DateTime? plannedShipDateFromUtc,
        DateTime? plannedShipDateToUtc,
        bool? isCrossBorder,
        bool? isPartialShipment)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(x =>
                x.ShipmentNumber.Contains(term) ||
                (x.CustomerReference != null && x.CustomerReference.Contains(term)) ||
                (x.ExternalReference != null && x.ExternalReference.Contains(term)) ||
                (x.TrackingNumber != null && x.TrackingNumber.Contains(term)) ||
                (x.MasterTrackingNumber != null && x.MasterTrackingNumber.Contains(term)));
        }

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (type.HasValue)
            query = query.Where(x => x.Type == type.Value);

        if (priority.HasValue)
            query = query.Where(x => x.Priority == priority.Value);

        if (orderId.HasValue)
            query = query.Where(x => x.OrderId == orderId.Value);

        if (warehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == warehouseId.Value);

        if (carrierId.HasValue)
            query = query.Where(x => x.CarrierId == carrierId.Value);

        if (plannedShipDateFromUtc.HasValue)
            query = query.Where(x => x.PlannedShipDateUtc >= plannedShipDateFromUtc.Value);

        if (plannedShipDateToUtc.HasValue)
            query = query.Where(x => x.PlannedShipDateUtc <= plannedShipDateToUtc.Value);

        if (isCrossBorder.HasValue)
            query = query.Where(x => x.IsCrossBorder == isCrossBorder.Value);

        if (isPartialShipment.HasValue)
            query = query.Where(x => x.IsPartialShipment == isPartialShipment.Value);

        return query;
    }

    public async Task AddItemAsync(ShipmentItem item, CancellationToken cancellationToken = default)
    => await _context.ShipmentItems.AddAsync(item, cancellationToken);

    public void UpdateItem(ShipmentItem item)
        => _context.ShipmentItems.Update(item);

    public void RemoveItem(ShipmentItem item)
        => _context.ShipmentItems.Remove(item);

    public async Task AddPackageAsync(ShipmentPackage package, CancellationToken cancellationToken = default)
        => await _context.ShipmentPackages.AddAsync(package, cancellationToken);

    public void UpdatePackage(ShipmentPackage package)
        => _context.ShipmentPackages.Update(package);

    public void RemovePackage(ShipmentPackage package)
        => _context.ShipmentPackages.Remove(package);

    public async Task AddPackageItemAsync(ShipmentPackageItem packageItem, CancellationToken cancellationToken = default)
        => await _context.ShipmentPackageItems.AddAsync(packageItem, cancellationToken);

    public void UpdatePackageItem(ShipmentPackageItem packageItem)
        => _context.ShipmentPackageItems.Update(packageItem);

    public void RemovePackageItem(ShipmentPackageItem packageItem)
        => _context.ShipmentPackageItems.Remove(packageItem);

    public async Task AddTrackingEventAsync(ShipmentTrackingEvent trackingEvent, CancellationToken cancellationToken = default)
        => await _context.ShipmentTrackingEvents.AddAsync(trackingEvent, cancellationToken);

    public async Task AddDocumentAsync(ShipmentDocument document, CancellationToken cancellationToken = default)
        => await _context.ShipmentDocuments.AddAsync(document, cancellationToken);

    public async Task AddChargeAsync(ShipmentCharge charge, CancellationToken cancellationToken = default)
        => await _context.ShipmentCharges.AddAsync(charge, cancellationToken);

    public async Task AddExceptionAsync(ShipmentException shipmentException, CancellationToken cancellationToken = default)
        => await _context.ShipmentExceptions.AddAsync(shipmentException, cancellationToken);

    public async Task AddInsuranceAsync(ShipmentInsurance insurance, CancellationToken cancellationToken = default)
        => await _context.ShipmentInsurances.AddAsync(insurance, cancellationToken);

    public async Task AddCustomsDocumentAsync(CustomsDocument customsDocument, CancellationToken cancellationToken = default)
        => await _context.CustomsDocuments.AddAsync(customsDocument, cancellationToken);

    public async Task AddStatusHistoryAsync(ShipmentStatusHistory statusHistory, CancellationToken cancellationToken = default)
        => await _context.ShipmentStatusHistories.AddAsync(statusHistory, cancellationToken);

}
