namespace OperationIntelligence.DB;

public interface IVendorBillRepository : IBaseRepository<VendorBill>
{
    Task<VendorBill?> GetByBillNumberAsync(string billNumber, CancellationToken cancellationToken = default);
    Task<VendorBill?> GetWithLinesAsync(Guid vendorBillId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VendorBill>> GetByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VendorBill>> GetByStatusAsync(VendorBillStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VendorBill>> GetOverdueBillsAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<decimal> GetOutstandingBalanceAsync(Guid vendorBillId, CancellationToken cancellationToken = default);
}
