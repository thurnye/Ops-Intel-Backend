using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class WorkCenterRepository : BaseRepository<WorkCenter>, IWorkCenterRepository
{
    public WorkCenterRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<WorkCenter?> GetWithMachinesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Machines)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkCenter>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkCenter>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.WarehouseId == warehouseId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.Code == code &&
                 !x.IsDeleted &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
