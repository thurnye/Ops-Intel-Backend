using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class OrderItemRepository : OrderBaseRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.UnitOfMeasure)
            .Where(x => x.OrderId == orderId && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Order)
            .Include(x => x.Product)
            .Include(x => x.UnitOfMeasure)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
    }
}