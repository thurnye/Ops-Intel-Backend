using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class BillOfMaterialItemRepository : BaseRepository<BillOfMaterialItem>, IBillOfMaterialItemRepository
{
    public BillOfMaterialItemRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<BillOfMaterialItem>> GetByBillOfMaterialIdAsync(Guid billOfMaterialId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.MaterialProduct)
            .Include(x => x.UnitOfMeasure)
            .Where(x => x.BillOfMaterialId == billOfMaterialId && !x.IsDeleted)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BillOfMaterialItem>> GetByMaterialProductIdAsync(Guid materialProductId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.MaterialProductId == materialProductId && !x.IsDeleted)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
    }

    public async Task<BillOfMaterialItem?> GetByBillOfMaterialAndSequenceAsync(Guid billOfMaterialId, int sequence, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.BillOfMaterialId == billOfMaterialId &&
                     x.Sequence == sequence &&
                     !x.IsDeleted,
                cancellationToken);
    }
}
