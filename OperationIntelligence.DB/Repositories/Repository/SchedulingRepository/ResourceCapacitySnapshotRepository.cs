using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ResourceCapacitySnapshotRepository : IResourceCapacitySnapshotRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ResourceCapacitySnapshotRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ResourceCapacitySnapshot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ResourceCapacitySnapshots
            .AsNoTracking()
            .Include(x => x.Shift)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<ResourceCapacitySnapshot> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var query = _context.ResourceCapacitySnapshots
            .AsNoTracking()
            .Include(x => x.Shift)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.SnapshotDateUtc);

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalRecords);
    }

    public async Task AddAsync(ResourceCapacitySnapshot entity, CancellationToken cancellationToken = default)
    {
        await _context.ResourceCapacitySnapshots.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ResourceCapacitySnapshot entity, CancellationToken cancellationToken = default)
    {
        _context.ResourceCapacitySnapshots.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
