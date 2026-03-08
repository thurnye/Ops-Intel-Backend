using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class OrderPaymentRepository : OrderBaseRepository<OrderPayment>, IOrderPaymentRepository
{
    public OrderPaymentRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<OrderPayment?> GetByPaymentReferenceAsync(string paymentReference, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.PaymentReference == paymentReference && x.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderPayment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.OrderId == orderId && x.IsActive)
            .OrderByDescending(x => x.PaymentDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderPayment>> GetSuccessfulPaymentsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x =>
                x.OrderId == orderId &&
                x.IsActive &&
                (x.Status == PaymentStatus.Paid ||
                 x.Status == PaymentStatus.PartiallyRefunded ||
                 x.Status == PaymentStatus.Refunded))
            .OrderByDescending(x => x.PaymentDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByPaymentReferenceAsync(string paymentReference, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(x => x.PaymentReference == paymentReference && x.IsActive, cancellationToken);
    }
}