using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class OrderNoteRepository : OrderBaseRepository<OrderNote>, IOrderNoteRepository
{
    public OrderNoteRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<OrderNote>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderNote>> GetInternalNotesByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.OrderId == orderId && x.IsInternal)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}