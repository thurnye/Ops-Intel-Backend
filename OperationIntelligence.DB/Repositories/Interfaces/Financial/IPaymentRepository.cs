namespace OperationIntelligence.DB;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<Payment?> GetByPaymentReferenceAsync(string paymentReference, CancellationToken cancellationToken = default);
    Task<Payment?> GetWithAllocationsAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
