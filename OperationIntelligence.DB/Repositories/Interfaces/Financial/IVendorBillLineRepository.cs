namespace OperationIntelligence.DB;

public interface IVendorBillLineRepository : IBaseRepository<VendorBillLine>
{
    Task<IReadOnlyList<VendorBillLine>> GetByVendorBillIdAsync(Guid vendorBillId, CancellationToken cancellationToken = default);
}
