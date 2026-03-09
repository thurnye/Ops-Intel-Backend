using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class DispatchQueueRepository : IDispatchQueueRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public DispatchQueueRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<DispatchQueueItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DispatchQueueItems
            .AsNoTracking()
            .Include(x => x.WorkCenter)
            .Include(x => x.Machine)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<DispatchQueueItem>> GetByWorkCenterAsync(Guid workCenterId, Guid? machineId = null, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.DispatchQueueItems
            .AsNoTracking()
            .Where(x => x.WorkCenterId == workCenterId && !x.IsDeleted);

        if (machineId.HasValue)
            query = query.Where(x => x.MachineId == machineId.Value);

        if (activeOnly)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.QueuePosition)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(DispatchQueueItem entity, CancellationToken cancellationToken = default)
    {
        await _context.DispatchQueueItems.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(DispatchQueueItem entity, CancellationToken cancellationToken = default)
    {
        _context.DispatchQueueItems.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DispatchQueueItem entity, CancellationToken cancellationToken = default)
    {
        _context.DispatchQueueItems.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsQueuePositionAsync(Guid workCenterId, Guid? machineId, int queuePosition, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.DispatchQueueItems.AnyAsync(x =>
            x.WorkCenterId == workCenterId &&
            x.MachineId == machineId &&
            x.QueuePosition == queuePosition &&
            !x.IsDeleted &&
            (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
