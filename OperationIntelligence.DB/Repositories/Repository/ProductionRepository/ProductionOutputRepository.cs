using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductionOutputRepository : BaseRepository<ProductionOutput>, IProductionOutputRepository
{
    public ProductionOutputRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductionOutput>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.UnitOfMeasure)
            .Where(x => x.ProductionOrderId == productionOrderId && !x.IsDeleted)
            .OrderBy(x => x.OutputDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionOutput>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductId == productId && !x.IsDeleted)
            .OrderByDescending(x => x.OutputDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionOutput>> GetFinalOutputsByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductionOrderId == productionOrderId && x.IsFinalOutput && !x.IsDeleted)
            .OrderBy(x => x.OutputDate)
            .ToListAsync(cancellationToken);
    }
}
