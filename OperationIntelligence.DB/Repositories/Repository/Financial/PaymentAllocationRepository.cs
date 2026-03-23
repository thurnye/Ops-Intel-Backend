using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class PaymentAllocationRepository : BaseRepository<PaymentAllocation>, IPaymentAllocationRepository
{
    public PaymentAllocationRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<PaymentAllocation>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.PaymentId == paymentId)
            .OrderByDescending(x => x.AllocationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentAllocation>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.InvoiceId == invoiceId)
            .OrderByDescending(x => x.AllocationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalAllocatedToInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.InvoiceId == invoiceId)
            .SumAsync(x => x.AmountApplied, cancellationToken);
    }
}
