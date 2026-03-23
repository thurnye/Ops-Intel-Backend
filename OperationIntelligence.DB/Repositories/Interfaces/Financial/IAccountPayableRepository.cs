namespace OperationIntelligence.DB;

public interface IAccountPayableRepository : IBaseRepository<AccountPayable>
{
    Task<AccountPayable?> GetByVendorBillIdAsync(Guid vendorBillId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountPayable>> GetVendorOpenItemsAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<decimal> GetVendorOutstandingBalanceAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountPayable>> GetAgingReportAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
}
