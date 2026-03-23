using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    public async Task<Invoice?> GetWithLinesAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == invoiceId, cancellationToken);
    }

    public async Task<Invoice?> GetWithAllocationsAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.PaymentAllocations)
            .FirstOrDefaultAsync(x => x.Id == invoiceId, cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetOverdueInvoicesAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.DueDate < asOfDate && x.OutstandingAmount > 0)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetOutstandingBalanceAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Id == invoiceId)
            .Select(x => x.OutstandingAmount)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
