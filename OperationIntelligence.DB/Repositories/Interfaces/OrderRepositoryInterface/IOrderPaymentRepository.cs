namespace OperationIntelligence.DB;

public interface IOrderPaymentRepository : IOrderBaseRepository<OrderPayment>
{
    Task<OrderPayment?> GetByPaymentReferenceAsync(string paymentReference, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderPayment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderPayment>> GetSuccessfulPaymentsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPaymentReferenceAsync(string paymentReference, CancellationToken cancellationToken = default);
}