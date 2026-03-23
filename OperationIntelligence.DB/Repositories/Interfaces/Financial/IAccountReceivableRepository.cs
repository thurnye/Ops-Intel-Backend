namespace OperationIntelligence.DB;

public interface IAccountReceivableRepository : IBaseRepository<AccountReceivable>
{
    Task<AccountReceivable?> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountReceivable>> GetCustomerOpenItemsAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<decimal> GetCustomerOutstandingBalanceAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountReceivable>> GetAgingReportAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
}
