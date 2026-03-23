using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class AccountPayableRepository : BaseRepository<AccountPayable>, IAccountPayableRepository
{
    public AccountPayableRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<AccountPayable?> GetByVendorBillIdAsync(Guid vendorBillId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.VendorBillId == vendorBillId, cancellationToken);
    }

    public async Task<IReadOnlyList<AccountPayable>> GetVendorOpenItemsAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.VendorId == vendorId && x.OutstandingAmount > 0)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetVendorOutstandingBalanceAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.VendorId == vendorId)
            .SumAsync(x => x.OutstandingAmount, cancellationToken);
    }

    public async Task<IReadOnlyList<AccountPayable>> GetAgingReportAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.OutstandingAmount > 0 && x.BillDate <= asOfDate)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }
}
