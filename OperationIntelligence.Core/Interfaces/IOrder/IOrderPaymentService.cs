namespace OperationIntelligence.Core;

public interface IOrderPaymentService
{
    Task<OrderPaymentResponse> RecordAsync(RecordOrderPaymentRequest request, CancellationToken cancellationToken = default);
    Task<OrderPaymentResponse> RefundAsync(RefundOrderPaymentRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderPaymentResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}