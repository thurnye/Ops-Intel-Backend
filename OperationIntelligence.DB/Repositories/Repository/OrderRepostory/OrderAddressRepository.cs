using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class OrderAddressRepository : OrderBaseRepository<OrderAddress>, IOrderAddressRepository
{
    public OrderAddressRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<OrderAddress>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.AddressType)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderAddress?> GetBillingAddressAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId && x.AddressType == AddressType.Billing,
                cancellationToken);
    }

    public async Task<OrderAddress?> GetShippingAddressAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId && x.AddressType == AddressType.Shipping,
                cancellationToken);
    }
}