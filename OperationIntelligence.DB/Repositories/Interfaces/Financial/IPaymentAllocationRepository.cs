namespace OperationIntelligence.DB;

public interface IPaymentAllocationRepository : IBaseRepository<PaymentAllocation>
{
    Task<IReadOnlyList<PaymentAllocation>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentAllocation>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAllocatedToInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
