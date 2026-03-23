using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class CostCenterRepository : BaseRepository<CostCenter>, ICostCenterRepository
{
    public CostCenterRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<CostCenter?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<CostCenter>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.Code == code && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
