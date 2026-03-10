using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductionLaborLogRepository : BaseRepository<ProductionLaborLog>, IProductionLaborLogRepository
{
    public ProductionLaborLogRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductionLaborLog>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductionExecutionId == productionExecutionId && !x.IsDeleted)
            .OrderByDescending(x => x.WorkDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionLaborLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.WorkDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionLaborLog>> GetByWorkDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x =>
                x.WorkDate >= startDate &&
                x.WorkDate <= endDate &&
                !x.IsDeleted)
            .OrderByDescending(x => x.WorkDate)
            .ToListAsync(cancellationToken);
    }
}
