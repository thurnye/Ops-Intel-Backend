using OperationIntelligence.DB;
using OrderPaymentStatus = global::PaymentStatus;

namespace OperationIntelligence.Core;

public class OrderPaymentService : IOrderPaymentService
{
    private readonly IOrderPaymentRepository _orderPaymentRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderPaymentService(
        IOrderPaymentRepository orderPaymentRepository,
        IOrderRepository orderRepository)
    {
        _orderPaymentRepository = orderPaymentRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderPaymentResponse> RecordAsync(RecordOrderPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        if (request.Amount <= 0)
            throw new InvalidOperationException(OrderErrorMessages.PaymentAmountMustBeGreaterThanZero);

        if (!string.Equals(order.CurrencyCode, request.CurrencyCode, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(OrderErrorMessages.PaymentCurrencyMustMatchOrder);

        var paymentReference = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        if (await _orderPaymentRepository.ExistsByPaymentReferenceAsync(paymentReference, cancellationToken))
            throw new InvalidOperationException(OrderErrorMessages.GeneratedPaymentReferenceAlreadyExists);

        var payment = new OrderPayment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            PaymentReference = paymentReference,
            ExternalTransactionId = request.ExternalTransactionId,
            ExternalPaymentIntentId = request.ExternalPaymentIntentId,
            PaymentMethod = request.PaymentMethod,
            PaymentProvider = request.PaymentProvider,
            TransactionType = PaymentTransactionType.Payment,
            Status = OrderPaymentStatus.Paid,
            Amount = request.Amount,
            FeeAmount = request.FeeAmount,
            NetAmount = request.Amount - request.FeeAmount,
            CurrencyCode = request.CurrencyCode,
            PaymentDateUtc = DateTime.UtcNow,
            ProcessedDateUtc = DateTime.UtcNow,
            PayerName = request.PayerName,
            PayerEmail = request.PayerEmail,
            Last4 = request.Last4,
            AuthorizationCode = request.AuthorizationCode,
            ReceiptNumber = request.ReceiptNumber,
            Notes = request.Notes,
            RecordedBy = request.RecordedBy,
            IsRefunded = false,
            RefundedAmount = 0m,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _orderPaymentRepository.AddAsync(payment, cancellationToken);

        order.PaidAmount += payment.Amount;
        order.OutstandingAmount = order.TotalAmount - order.PaidAmount + order.RefundedAmount;
        order.PaymentStatus = CalculatePaymentStatus(order.TotalAmount, order.PaidAmount, order.RefundedAmount);
        order.UpdatedAtUtc = DateTime.UtcNow;

        _orderRepository.Update(order);

        await _orderPaymentRepository.SaveChangesAsync(cancellationToken);

        return new OrderPaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            PaymentReference = payment.PaymentReference,
            PaymentMethod = payment.PaymentMethod,
            PaymentProvider = payment.PaymentProvider,
            TransactionType = payment.TransactionType,
            Status = payment.Status,
            Amount = payment.Amount,
            FeeAmount = payment.FeeAmount,
            NetAmount = payment.NetAmount,
            RefundedAmount = payment.RefundedAmount,
            CurrencyCode = payment.CurrencyCode,
            PaymentDateUtc = payment.PaymentDateUtc
        };
    }

    public async Task<OrderPaymentResponse> RefundAsync(RefundOrderPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var payment = await _orderPaymentRepository.GetByIdAsync(request.OrderPaymentId, cancellationToken);
        if (payment == null || !payment.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.PaymentNotFound);

        var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        var refundableAmount = payment.Amount - payment.RefundedAmount;
        if (request.RefundAmount <= 0 || request.RefundAmount > refundableAmount)
            throw new InvalidOperationException(OrderErrorMessages.InvalidRefundAmount);

        payment.RefundedAmount += request.RefundAmount;
        payment.IsRefunded = payment.RefundedAmount > 0;
        payment.Status = payment.RefundedAmount >= payment.Amount
            ? OrderPaymentStatus.Refunded
            : OrderPaymentStatus.PartiallyRefunded;
        payment.UpdatedAtUtc = DateTime.UtcNow;

        order.RefundedAmount += request.RefundAmount;
        order.OutstandingAmount = order.TotalAmount - order.PaidAmount + order.RefundedAmount;
        order.PaymentStatus = CalculatePaymentStatus(order.TotalAmount, order.PaidAmount, order.RefundedAmount);
        order.UpdatedAtUtc = DateTime.UtcNow;

        _orderPaymentRepository.Update(payment);
        _orderRepository.Update(order);

        await _orderPaymentRepository.SaveChangesAsync(cancellationToken);

        return new OrderPaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            PaymentReference = payment.PaymentReference,
            PaymentMethod = payment.PaymentMethod,
            PaymentProvider = payment.PaymentProvider,
            TransactionType = payment.TransactionType,
            Status = payment.Status,
            Amount = payment.Amount,
            FeeAmount = payment.FeeAmount,
            NetAmount = payment.NetAmount,
            RefundedAmount = payment.RefundedAmount,
            CurrencyCode = payment.CurrencyCode,
            PaymentDateUtc = payment.PaymentDateUtc
        };
    }

    public async Task<IReadOnlyList<OrderPaymentResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var rows = await _orderPaymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return rows.Select(payment => new OrderPaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            PaymentReference = payment.PaymentReference,
            PaymentMethod = payment.PaymentMethod,
            PaymentProvider = payment.PaymentProvider,
            TransactionType = payment.TransactionType,
            Status = payment.Status,
            Amount = payment.Amount,
            FeeAmount = payment.FeeAmount,
            NetAmount = payment.NetAmount,
            RefundedAmount = payment.RefundedAmount,
            CurrencyCode = payment.CurrencyCode,
            PaymentDateUtc = payment.PaymentDateUtc
        }).ToList();
    }

    private static OrderPaymentStatus CalculatePaymentStatus(decimal total, decimal paid, decimal refunded)
    {
        var effectivePaid = paid - refunded;

        if (effectivePaid <= 0)
            return OrderPaymentStatus.Unpaid;

        if (refunded > 0 && refunded < paid)
            return OrderPaymentStatus.PartiallyRefunded;

        if (paid > 0 && refunded >= paid)
            return OrderPaymentStatus.Refunded;

        if (effectivePaid < total)
            return OrderPaymentStatus.PartiallyPaid;

        return OrderPaymentStatus.Paid;
    }
}
