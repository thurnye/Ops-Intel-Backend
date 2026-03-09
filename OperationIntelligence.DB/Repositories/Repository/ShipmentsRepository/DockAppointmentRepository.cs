using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class DockAppointmentRepository : BaseRepository<DockAppointment>, IDockAppointmentRepository
{
    public DockAppointmentRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<DockAppointment?> GetByIdWithShipmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Carrier)
            .Include(x => x.Shipments)
                .ThenInclude(x => x.Order)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<DockAppointment?> GetByAppointmentNumberAsync(string appointmentNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppointmentNumber == appointmentNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<DockAppointment>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        DockAppointmentStatus? status = null,
        Guid? warehouseId = null,
        Guid? carrierId = null,
        DateTime? scheduledStartFromUtc = null,
        DateTime? scheduledStartToUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildDockAppointmentFilterQuery(
            _dbSet.AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Carrier),
            search,
            status,
            warehouseId,
            carrierId,
            scheduledStartFromUtc,
            scheduledStartToUtc);

        return await query
            .OrderBy(x => x.ScheduledStartUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? search = null,
        DockAppointmentStatus? status = null,
        Guid? warehouseId = null,
        Guid? carrierId = null,
        DateTime? scheduledStartFromUtc = null,
        DateTime? scheduledStartToUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildDockAppointmentFilterQuery(
            _dbSet.AsNoTracking(),
            search,
            status,
            warehouseId,
            carrierId,
            scheduledStartFromUtc,
            scheduledStartToUtc);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DockAppointment>> GetUpcomingAsync(
        Guid? warehouseId = null,
        DateTime? fromUtc = null,
        CancellationToken cancellationToken = default)
    {
        var effectiveFromUtc = fromUtc ?? DateTime.UtcNow;

        var query = _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Carrier)
            .Where(x => x.ScheduledStartUtc >= effectiveFromUtc);

        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        return await query
            .OrderBy(x => x.ScheduledStartUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasDockConflictAsync(
        Guid warehouseId,
        string dockCode,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        Guid? excludeAppointmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking()
            .Where(x =>
                x.WarehouseId == warehouseId &&
                x.DockCode == dockCode &&
                x.Status != DockAppointmentStatus.Cancelled &&
                x.Status != DockAppointmentStatus.Completed &&
                x.ScheduledStartUtc < scheduledEndUtc &&
                x.ScheduledEndUtc > scheduledStartUtc);

        if (excludeAppointmentId.HasValue)
        {
            query = query.Where(x => x.Id != excludeAppointmentId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    private static IQueryable<DockAppointment> BuildDockAppointmentFilterQuery(
        IQueryable<DockAppointment> query,
        string? search,
        DockAppointmentStatus? status,
        Guid? warehouseId,
        Guid? carrierId,
        DateTime? scheduledStartFromUtc,
        DateTime? scheduledStartToUtc)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(x =>
                x.AppointmentNumber.Contains(term) ||
                (x.DockCode != null && x.DockCode.Contains(term)) ||
                (x.TrailerNumber != null && x.TrailerNumber.Contains(term)) ||
                (x.DriverName != null && x.DriverName.Contains(term)));
        }

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (warehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == warehouseId.Value);

        if (carrierId.HasValue)
            query = query.Where(x => x.CarrierId == carrierId.Value);

        if (scheduledStartFromUtc.HasValue)
            query = query.Where(x => x.ScheduledStartUtc >= scheduledStartFromUtc.Value);

        if (scheduledStartToUtc.HasValue)
            query = query.Where(x => x.ScheduledStartUtc <= scheduledStartToUtc.Value);

        return query;
    }
}
