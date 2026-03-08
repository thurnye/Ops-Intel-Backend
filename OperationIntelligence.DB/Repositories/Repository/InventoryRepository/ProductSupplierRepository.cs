using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductSupplierRepository : BaseRepository<ProductSupplier>, IProductSupplierRepository
{
    public ProductSupplierRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductSupplier>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Supplier)
            .Where(x => x.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductSupplier?> GetByProductAndSupplierAsync(Guid productId, Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.SupplierId == supplierId, cancellationToken);
    }
}
