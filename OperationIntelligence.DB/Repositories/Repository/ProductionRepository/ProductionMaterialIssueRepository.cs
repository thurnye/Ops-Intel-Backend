using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductionMaterialIssueRepository : BaseRepository<ProductionMaterialIssue>, IProductionMaterialIssueRepository
{
    public ProductionMaterialIssueRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductionMaterialIssue>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.MaterialProduct)
            .Include(x => x.UnitOfMeasure)
            .Where(x => x.ProductionOrderId == productionOrderId && !x.IsDeleted)
            .OrderBy(x => x.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionMaterialIssue>> GetByMaterialProductIdAsync(Guid materialProductId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.MaterialProductId == materialProductId && !x.IsDeleted)
            .OrderByDescending(x => x.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductionMaterialIssue?> GetWithConsumptionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.MaterialProduct)
            .Include(x => x.UnitOfMeasure)
            .Include(x => x.Consumptions)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }
}
