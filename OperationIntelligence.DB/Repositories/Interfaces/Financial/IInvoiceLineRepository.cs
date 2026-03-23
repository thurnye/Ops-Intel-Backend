namespace OperationIntelligence.DB;

public interface IInvoiceLineRepository : IBaseRepository<InvoiceLine>
{
    Task<IReadOnlyList<InvoiceLine>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
