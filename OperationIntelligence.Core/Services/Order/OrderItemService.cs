using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class OrderItemService : IOrderItemService
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderItemService(
        IOrderItemRepository orderItemRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _orderItemRepository = orderItemRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderItemResponse> AddAsync(CreateOrderItemRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.PendingApproval)
            throw new InvalidOperationException(OrderErrorMessages.ItemsCanOnlyBeAddedToDraftOrPending);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null || !product.IsDeleted)
            throw new KeyNotFoundException(OrderErrorMessages.ProductNotFound);

        if (request.QuantityOrdered <= 0)
            throw new InvalidOperationException(OrderErrorMessages.ItemQuantityMustBeGreaterThanZero);

        var lineSubtotal = request.QuantityOrdered * request.UnitPrice;
        var lineTotal = lineSubtotal - request.DiscountAmount + request.TaxAmount;

        var entity = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            ProductId = request.ProductId,
            UnitOfMeasureId = request.UnitOfMeasureId,
            ProductNameSnapshot = product.Name,
            ProductSkuSnapshot = product.SKU,
            ProductDescriptionSnapshot = product.Description,
            QuantityOrdered = request.QuantityOrdered,
            QuantityAllocated = 0,
            QuantityShipped = 0,
            QuantityDelivered = 0,
            QuantityCancelled = 0,
            UnitPrice = request.UnitPrice,
            DiscountAmount = request.DiscountAmount,
            TaxAmount = request.TaxAmount,
            LineTotal = lineTotal,
            Remarks = request.Remarks,
            SortOrder = request.SortOrder,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _orderItemRepository.AddAsync(entity, cancellationToken);

        order.SubtotalAmount += lineSubtotal;
        order.DiscountAmount += request.DiscountAmount;
        order.TaxAmount += request.TaxAmount;
        order.TotalAmount = order.SubtotalAmount - order.DiscountAmount + order.TaxAmount + order.ShippingAmount;
        order.OutstandingAmount = order.TotalAmount - order.PaidAmount + order.RefundedAmount;

        _orderRepository.Update(order);

        await _orderItemRepository.SaveChangesAsync(cancellationToken);

        return new OrderItemResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            ProductId = entity.ProductId,
            ProductNameSnapshot = entity.ProductNameSnapshot,
            ProductSkuSnapshot = entity.ProductSkuSnapshot,
            QuantityOrdered = entity.QuantityOrdered,
            UnitPrice = entity.UnitPrice,
            DiscountAmount = entity.DiscountAmount,
            TaxAmount = entity.TaxAmount,
            LineTotal = entity.LineTotal
        };
    }

    public async Task<OrderItemResponse> UpdateAsync(Guid id, UpdateOrderItemRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _orderItemRepository.GetByIdAsync(id, cancellationToken);
        if (item == null || !item.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderItemNotFound);

        var order = await _orderRepository.GetByIdAsync(item.OrderId, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.PendingApproval)
            throw new InvalidOperationException(OrderErrorMessages.ItemsCanOnlyBeUpdatedOnDraftOrPending);

        var oldLineSubtotal = item.QuantityOrdered * item.UnitPrice;

        item.QuantityOrdered = request.QuantityOrdered;
        item.UnitPrice = request.UnitPrice;
        item.DiscountAmount = request.DiscountAmount;
        item.TaxAmount = request.TaxAmount;
        item.Remarks = request.Remarks;
        item.SortOrder = request.SortOrder;
        item.LineTotal = (request.QuantityOrdered * request.UnitPrice) - request.DiscountAmount + request.TaxAmount;
        item.UpdatedAtUtc = DateTime.UtcNow;

        var newLineSubtotal = item.QuantityOrdered * item.UnitPrice;

        order.SubtotalAmount = order.SubtotalAmount - oldLineSubtotal + newLineSubtotal;
        order.TotalAmount = order.SubtotalAmount - order.DiscountAmount + order.TaxAmount + order.ShippingAmount;
        order.OutstandingAmount = order.TotalAmount - order.PaidAmount + order.RefundedAmount;
        order.UpdatedAtUtc = DateTime.UtcNow;

        _orderItemRepository.Update(item);
        _orderRepository.Update(order);

        await _orderItemRepository.SaveChangesAsync(cancellationToken);

        return new OrderItemResponse
        {
            Id = item.Id,
            OrderId = item.OrderId,
            ProductId = item.ProductId,
            ProductNameSnapshot = item.ProductNameSnapshot,
            ProductSkuSnapshot = item.ProductSkuSnapshot,
            QuantityOrdered = item.QuantityOrdered,
            UnitPrice = item.UnitPrice,
            DiscountAmount = item.DiscountAmount,
            TaxAmount = item.TaxAmount,
            LineTotal = item.LineTotal
        };
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _orderItemRepository.GetByIdAsync(id, cancellationToken);
        if (item == null || !item.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderItemNotFound);

        var order = await _orderRepository.GetByIdAsync(item.OrderId, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.PendingApproval)
            throw new InvalidOperationException(OrderErrorMessages.ItemsCanOnlyBeRemovedFromDraftOrPending);

        item.IsActive = false;
        item.UpdatedAtUtc = DateTime.UtcNow;

        order.SubtotalAmount -= item.QuantityOrdered * item.UnitPrice;
        order.DiscountAmount -= item.DiscountAmount;
        order.TaxAmount -= item.TaxAmount;
        order.TotalAmount = order.SubtotalAmount - order.DiscountAmount + order.TaxAmount + order.ShippingAmount;
        order.OutstandingAmount = order.TotalAmount - order.PaidAmount + order.RefundedAmount;
        order.UpdatedAtUtc = DateTime.UtcNow;

        _orderItemRepository.Update(item);
        _orderRepository.Update(order);

        await _orderItemRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItemResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var items = await _orderItemRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return items.Select(item => new OrderItemResponse
        {
            Id = item.Id,
            OrderId = item.OrderId,
            ProductId = item.ProductId,
            ProductNameSnapshot = item.ProductNameSnapshot,
            ProductSkuSnapshot = item.ProductSkuSnapshot,
            QuantityOrdered = item.QuantityOrdered,
            UnitPrice = item.UnitPrice,
            DiscountAmount = item.DiscountAmount,
            TaxAmount = item.TaxAmount,
            LineTotal = item.LineTotal
        }).ToList();
    }
}