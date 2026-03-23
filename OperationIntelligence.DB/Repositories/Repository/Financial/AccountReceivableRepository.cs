using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class AccountReceivableRepository : BaseRepository<AccountReceivable>, IAccountReceivableRepository
{
    public AccountReceivableRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<AccountReceivable?> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.InvoiceId == invoiceId, cancellationToken);
    }

    public async Task<IReadOnlyList<AccountReceivable>> GetCustomerOpenItemsAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.OutstandingAmount > 0)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetCustomerOutstandingBalanceAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .SumAsync(x => x.OutstandingAmount, cancellationToken);
    }

    public async Task<IReadOnlyList<AccountReceivable>> GetAgingReportAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.OutstandingAmount > 0 && x.InvoiceDate <= asOfDate)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }
}
