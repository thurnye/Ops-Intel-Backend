using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class VendorBillLineRepository : BaseRepository<VendorBillLine>, IVendorBillLineRepository
{
    public VendorBillLineRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<VendorBillLine>> GetByVendorBillIdAsync(Guid vendorBillId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.VendorBillId == vendorBillId)
            .OrderBy(x => x.LineNumber)
            .ToListAsync(cancellationToken);
    }
}
