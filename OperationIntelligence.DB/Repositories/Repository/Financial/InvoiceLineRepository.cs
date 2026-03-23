using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class InvoiceLineRepository : BaseRepository<InvoiceLine>, IInvoiceLineRepository
{
    public InvoiceLineRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<InvoiceLine>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.InvoiceId == invoiceId)
            .OrderBy(x => x.LineNumber)
            .ToListAsync(cancellationToken);
    }
}
