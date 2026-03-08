using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SKU == sku, cancellationToken);
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Barcode == barcode, cancellationToken);
    }

    public async Task<Product?> GetProductWithImagesAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Images.Where(i => !i.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);
    }

    public async Task<Product?> GetProductWithDetailsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .Include(x => x.UnitOfMeasure)
            .Include(x => x.Images.Where(i => !i.IsDeleted))
            .Include(x => x.InventoryStocks.Where(s => !s.IsDeleted))
                .ThenInclude(s => s.Warehouse)
            .Include(x => x.ProductSuppliers.Where(ps => !ps.IsDeleted))
                .ThenInclude(ps => ps.Supplier)
            .FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        ProductStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(x =>
                x.Name.Contains(term) ||
                x.SKU.Contains(term) ||
                (x.Barcode != null && x.Barcode.Contains(term)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? searchTerm = null,
        Guid? categoryId = null,
        ProductStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(x =>
                x.Name.Contains(term) ||
                x.SKU.Contains(term) ||
                (x.Barcode != null && x.Barcode.Contains(term)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query.CountAsync(cancellationToken);
    }
    public async Task<bool> IsSkuUniqueAsync(
        string sku,
        Guid? excludeProductId = null,
        CancellationToken cancellationToken = default)
    {
        return !await _dbSet.AnyAsync(x =>
            x.SKU == sku &&
            (!excludeProductId.HasValue || x.Id != excludeProductId.Value),
            cancellationToken);
    }

    public async Task<bool> IsBarcodeUniqueAsync(
        string? barcode,
        Guid? excludeProductId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return true;

        return !await _dbSet.AnyAsync(x =>
            x.Barcode == barcode &&
            (!excludeProductId.HasValue || x.Id != excludeProductId.Value),
            cancellationToken);
    }
}
