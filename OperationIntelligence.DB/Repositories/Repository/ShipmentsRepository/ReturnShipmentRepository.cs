using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ReturnShipmentRepository : BaseRepository<ReturnShipment>, IReturnShipmentRepository
{
    public ReturnShipmentRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<ReturnShipment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Shipment)
            .Include(x => x.Order)
            .Include(x => x.OriginAddress)
            .Include(x => x.DestinationAddress)
            .Include(x => x.Carrier)
            .Include(x => x.CarrierService)
            .Include(x => x.Items)
                .ThenInclude(x => x.ShipmentItem)
                    .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ReturnShipment?> GetByReturnShipmentNumberAsync(string returnShipmentNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ReturnShipmentNumber == returnShipmentNumber, cancellationToken);
    }

    public async Task<ReturnShipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TrackingNumber == trackingNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<ReturnShipment>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        ReturnShipmentStatus? status = null,
        Guid? shipmentId = null,
        Guid? orderId = null,
        Guid? carrierId = null,
        DateTime? requestedFromUtc = null,
        DateTime? requestedToUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildReturnShipmentFilterQuery(
            _dbSet.AsNoTracking()
                .Include(x => x.Shipment)
                .Include(x => x.Order)
                .Include(x => x.Carrier),
            search,
            status,
            shipmentId,
            orderId,
            carrierId,
            requestedFromUtc,
            requestedToUtc);

        return await query
            .OrderByDescending(x => x.RequestedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? search = null,
        ReturnShipmentStatus? status = null,
        Guid? shipmentId = null,
        Guid? orderId = null,
        Guid? carrierId = null,
        DateTime? requestedFromUtc = null,
        DateTime? requestedToUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildReturnShipmentFilterQuery(
            _dbSet.AsNoTracking(),
            search,
            status,
            shipmentId,
            orderId,
            carrierId,
            requestedFromUtc,
            requestedToUtc);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReturnShipment>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ShipmentId == shipmentId)
            .OrderByDescending(x => x.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReturnShipment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReturnShipmentItem?> GetReturnItemByIdAsync(Guid returnShipmentItemId, CancellationToken cancellationToken = default)
    {
        return await _context.ReturnShipmentItems
            .AsNoTracking()
            .Include(x => x.ReturnShipment)
            .Include(x => x.ShipmentItem)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == returnShipmentItemId, cancellationToken);
    }

    public async Task<IReadOnlyList<ReturnShipmentItem>> GetItemsByReturnShipmentIdAsync(Guid returnShipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.ReturnShipmentItems
            .AsNoTracking()
            .Include(x => x.ShipmentItem)
                .ThenInclude(x => x.Product)
            .Where(x => x.ReturnShipmentId == returnShipmentId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalReturnedQuantityForShipmentItemAsync(Guid shipmentItemId, CancellationToken cancellationToken = default)
    {
        return await _context.ReturnShipmentItems
            .AsNoTracking()
            .Where(x => x.ShipmentItemId == shipmentItemId)
            .SumAsync(x => (decimal?)x.ReturnedQuantity, cancellationToken) ?? 0m;
    }

    private static IQueryable<ReturnShipment> BuildReturnShipmentFilterQuery(
        IQueryable<ReturnShipment> query,
        string? search,
        ReturnShipmentStatus? status,
        Guid? shipmentId,
        Guid? orderId,
        Guid? carrierId,
        DateTime? requestedFromUtc,
        DateTime? requestedToUtc)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(x =>
                x.ReturnShipmentNumber.Contains(term) ||
                x.ReasonCode.Contains(term) ||
                (x.ReasonDescription != null && x.ReasonDescription.Contains(term)) ||
                (x.TrackingNumber != null && x.TrackingNumber.Contains(term)));
        }

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (shipmentId.HasValue)
            query = query.Where(x => x.ShipmentId == shipmentId.Value);

        if (orderId.HasValue)
            query = query.Where(x => x.OrderId == orderId.Value);

        if (carrierId.HasValue)
            query = query.Where(x => x.CarrierId == carrierId.Value);

        if (requestedFromUtc.HasValue)
            query = query.Where(x => x.RequestedAtUtc >= requestedFromUtc.Value);

        if (requestedToUtc.HasValue)
            query = query.Where(x => x.RequestedAtUtc <= requestedToUtc.Value);

        return query;
    }

    public async Task AddItemAsync(ReturnShipmentItem item, CancellationToken cancellationToken = default)
    => await _context.ReturnShipmentItems.AddAsync(item, cancellationToken);

public void UpdateItem(ReturnShipmentItem item)
    => _context.ReturnShipmentItems.Update(item);

public void RemoveItem(ReturnShipmentItem item)
    => _context.ReturnShipmentItems.Remove(item);
}
