using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class OrderStatusHistoryRepository : OrderBaseRepository<OrderStatusHistory>, IOrderStatusHistoryRepository
{
    public OrderStatusHistoryRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<OrderStatusHistory>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.ChangedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderStatusHistory?> GetLatestByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.ChangedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }
}