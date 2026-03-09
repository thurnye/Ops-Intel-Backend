using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductionScrapRepository : BaseRepository<ProductionScrap>, IProductionScrapRepository
{
    public ProductionScrapRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductionScrap>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductionOrderId == productionOrderId && !x.IsDeleted)
            .OrderByDescending(x => x.ScrapDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionScrap>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductionExecutionId == productionExecutionId && !x.IsDeleted)
            .OrderByDescending(x => x.ScrapDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionScrap>> GetByReasonAsync(ScrapReasonType reason, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.Reason == reason && !x.IsDeleted)
            .OrderByDescending(x => x.ScrapDate)
            .ToListAsync(cancellationToken);
    }
}
