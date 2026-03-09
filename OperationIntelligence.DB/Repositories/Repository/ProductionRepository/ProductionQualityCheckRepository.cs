using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductionQualityCheckRepository : BaseRepository<ProductionQualityCheck>, IProductionQualityCheckRepository
{
    public ProductionQualityCheckRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductionQualityCheck>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductionOrderId == productionOrderId && !x.IsDeleted)
            .OrderByDescending(x => x.CheckDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionQualityCheck>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductionExecutionId == productionExecutionId && !x.IsDeleted)
            .OrderByDescending(x => x.CheckDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionQualityCheck>> GetByStatusAsync(QualityCheckStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.Status == status && !x.IsDeleted)
            .OrderByDescending(x => x.CheckDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionQualityCheck>> GetByCheckTypeAsync(QualityCheckType checkType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.CheckType == checkType && !x.IsDeleted)
            .OrderByDescending(x => x.CheckDate)
            .ToListAsync(cancellationToken);
    }
}
