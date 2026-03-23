using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class VendorBillRepository : BaseRepository<VendorBill>, IVendorBillRepository
{
    public VendorBillRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<VendorBill?> GetByBillNumberAsync(string billNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.BillNumber == billNumber, cancellationToken);
    }

    public async Task<VendorBill?> GetWithLinesAsync(Guid vendorBillId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == vendorBillId, cancellationToken);
    }

    public async Task<IReadOnlyList<VendorBill>> GetByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.VendorId == vendorId)
            .OrderByDescending(x => x.BillDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VendorBill>> GetByStatusAsync(VendorBillStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.BillDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VendorBill>> GetOverdueBillsAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.DueDate < asOfDate && x.OutstandingAmount > 0)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetOutstandingBalanceAsync(Guid vendorBillId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Id == vendorBillId)
            .Select(x => x.OutstandingAmount)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
