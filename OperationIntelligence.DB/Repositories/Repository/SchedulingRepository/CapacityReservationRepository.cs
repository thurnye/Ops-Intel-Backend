using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;
public class CapacityReservationRepository : ICapacityReservationRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public CapacityReservationRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<CapacityReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CapacityReservations
            .AsNoTracking()
            .Include(x => x.Shift)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<CapacityReservation>> GetByOperationIdAsync(Guid scheduleOperationId, CancellationToken cancellationToken = default)
    {
        return await _context.CapacityReservations
            .AsNoTracking()
            .Include(x => x.Shift)
            .Where(x => x.ScheduleOperationId == scheduleOperationId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CapacityReservation>> GetByResourceAsync(Guid resourceId, ResourceType resourceType, DateTime? startUtc = null, DateTime? endUtc = null, CancellationToken cancellationToken = default)
    {
        var query = _context.CapacityReservations
            .AsNoTracking()
            .Where(x => x.ResourceId == resourceId && x.ResourceType == resourceType && !x.IsDeleted);

        if (startUtc.HasValue)
            query = query.Where(x => x.ReservedEndUtc >= startUtc.Value);

        if (endUtc.HasValue)
            query = query.Where(x => x.ReservedStartUtc <= endUtc.Value);

        return await query
            .OrderBy(x => x.ReservedStartUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CapacityReservation entity, CancellationToken cancellationToken = default)
    {
        await _context.CapacityReservations.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CapacityReservation entity, CancellationToken cancellationToken = default)
    {
        _context.CapacityReservations.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CapacityReservation entity, CancellationToken cancellationToken = default)
    {
        _context.CapacityReservations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasOverlapAsync(
        Guid resourceId,
        ResourceType resourceType,
        DateTime reservedStartUtc,
        DateTime reservedEndUtc,
        Guid? excludeReservationId = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.CapacityReservations.AnyAsync(x =>
            x.ResourceId == resourceId &&
            x.ResourceType == resourceType &&
            !x.IsDeleted &&
            (!excludeReservationId.HasValue || x.Id != excludeReservationId.Value) &&
            x.ReservedStartUtc < reservedEndUtc &&
            x.ReservedEndUtc > reservedStartUtc,
            cancellationToken);
    }
}
