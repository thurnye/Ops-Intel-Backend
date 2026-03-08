using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class OrderRepository : OrderBaseRepository<Order>, IOrderRepository
{
    public OrderRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrderNumber == orderNumber && x.IsActive, cancellationToken);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(i => i.Product)
            .Include(x => x.Items)
                .ThenInclude(i => i.UnitOfMeasure)
            .Include(x => x.Images)
            .Include(x => x.Addresses)
            .Include(x => x.OrderNotes)
            .Include(x => x.StatusHistory)
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
    }

    public async Task<Order?> GetByOrderNumberWithDetailsAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(i => i.Product)
            .Include(x => x.Items)
                .ThenInclude(i => i.UnitOfMeasure)
            .Include(x => x.Images)
            .Include(x => x.Addresses)
            .Include(x => x.OrderNotes)
            .Include(x => x.StatusHistory)
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.OrderNumber == orderNumber && x.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        OrderStatus? status = null,
        OrderType? orderType = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => x.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            query = query.Where(x =>
                x.OrderNumber.Contains(term) ||
                (x.CustomerName != null && x.CustomerName.Contains(term)) ||
                (x.CustomerEmail != null && x.CustomerEmail.Contains(term)) ||
                (x.ReferenceNumber != null && x.ReferenceNumber.Contains(term)) ||
                (x.CustomerPurchaseOrderNumber != null && x.CustomerPurchaseOrderNumber.Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (orderType.HasValue)
        {
            query = query.Where(x => x.OrderType == orderType.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? searchTerm = null,
        OrderStatus? status = null,
        OrderType? orderType = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(x => x.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            query = query.Where(x =>
                x.OrderNumber.Contains(term) ||
                (x.CustomerName != null && x.CustomerName.Contains(term)) ||
                (x.CustomerEmail != null && x.CustomerEmail.Contains(term)) ||
                (x.ReferenceNumber != null && x.ReferenceNumber.Contains(term)) ||
                (x.CustomerPurchaseOrderNumber != null && x.CustomerPurchaseOrderNumber.Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (orderType.HasValue)
        {
            query = query.Where(x => x.OrderType == orderType.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(x => x.OrderNumber == orderNumber && x.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => x.CustomerId == customerId && x.IsActive)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == warehouseId && x.IsActive)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}