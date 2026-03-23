namespace OperationIntelligence.DB;

public interface IInvoiceRepository : IBaseRepository<Invoice>
{
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Invoice?> GetWithLinesAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetWithAllocationsAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetOverdueInvoicesAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<decimal> GetOutstandingBalanceAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
