using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class OrderImageRepository : OrderBaseRepository<OrderImage>, IOrderImageRepository
{
    public OrderImageRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<OrderImage>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.OrderId == orderId && x.IsActive)
            .OrderByDescending(x => x.IsPrimary)
            .ThenByDescending(x => x.UploadedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderImage?> GetPrimaryImageByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrderId == orderId && x.IsPrimary && x.IsActive, cancellationToken);
    }
}