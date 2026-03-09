using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class DeliveryRunRepository : BaseRepository<DeliveryRun>, IDeliveryRunRepository
{
    public DeliveryRunRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<DeliveryRun?> GetByIdWithShipmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Shipments)
                .ThenInclude(x => x.Order)
            .Include(x => x.Shipments)
                .ThenInclude(x => x.Carrier)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<DeliveryRun?> GetByRunNumberAsync(string runNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RunNumber == runNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryRun>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        DeliveryRunStatus? status = null,
        Guid? warehouseId = null,
        DateTime? plannedStartFromUtc = null,
        DateTime? plannedStartToUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildDeliveryRunFilterQuery(
            _dbSet.AsNoTracking().Include(x => x.Warehouse),
            search,
            status,
            warehouseId,
            plannedStartFromUtc,
            plannedStartToUtc);

        return await query
            .OrderByDescending(x => x.PlannedStartUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? search = null,
        DeliveryRunStatus? status = null,
        Guid? warehouseId = null,
        DateTime? plannedStartFromUtc = null,
        DateTime? plannedStartToUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildDeliveryRunFilterQuery(
            _dbSet.AsNoTracking(),
            search,
            status,
            warehouseId,
            plannedStartFromUtc,
            plannedStartToUtc);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryRun>> GetActiveRunsAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var activeStatuses = new[]
        {
            DeliveryRunStatus.Planned,
            DeliveryRunStatus.Dispatched,
            DeliveryRunStatus.InProgress
        };

        var query = _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => activeStatuses.Contains(x.Status));

        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        return await query
            .OrderBy(x => x.PlannedStartUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetAssignedShipmentsAsync(Guid deliveryRunId, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .AsNoTracking()
            .Include(x => x.Order)
            .Include(x => x.Carrier)
            .Where(x => x.DeliveryRunId == deliveryRunId)
            .OrderBy(x => x.PlannedDeliveryDateUtc)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAssignedShipmentsAsync(Guid deliveryRunId, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .AsNoTracking()
            .CountAsync(x => x.DeliveryRunId == deliveryRunId, cancellationToken);
    }

    private static IQueryable<DeliveryRun> BuildDeliveryRunFilterQuery(
        IQueryable<DeliveryRun> query,
        string? search,
        DeliveryRunStatus? status,
        Guid? warehouseId,
        DateTime? plannedStartFromUtc,
        DateTime? plannedStartToUtc)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(x =>
                x.RunNumber.Contains(term) ||
                x.Name.Contains(term) ||
                (x.DriverName != null && x.DriverName.Contains(term)) ||
                (x.VehicleNumber != null && x.VehicleNumber.Contains(term)) ||
                (x.RouteCode != null && x.RouteCode.Contains(term)));
        }

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (warehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == warehouseId.Value);

        if (plannedStartFromUtc.HasValue)
            query = query.Where(x => x.PlannedStartUtc >= plannedStartFromUtc.Value);

        if (plannedStartToUtc.HasValue)
            query = query.Where(x => x.PlannedStartUtc <= plannedStartToUtc.Value);

        return query;
    }
}
