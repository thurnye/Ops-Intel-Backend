using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByPaymentReferenceAsync(string paymentReference, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PaymentReference == paymentReference, cancellationToken);
    }

    public async Task<Payment?> GetWithAllocationsAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Allocations)
            .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.PaymentDate >= from && x.PaymentDate <= to)
            .OrderByDescending(x => x.PaymentDate)
            .ToListAsync(cancellationToken);
    }
}
