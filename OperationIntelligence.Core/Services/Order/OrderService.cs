using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;
using OrderPaymentStatus = global::PaymentStatus;

namespace OperationIntelligence.Core;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new InvalidOperationException(OrderErrorMessages.OrderMustContainAtLeastOneItem);

        if (request.WarehouseId.HasValue)
        {
            var warehouseExists = await _warehouseRepository.ExistsAsync(x => x.Id == request.WarehouseId.Value && x.IsActive, cancellationToken);
            if (!warehouseExists)
                throw new KeyNotFoundException(OrderErrorMessages.WarehouseNotFound);
        }

        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        if (await _orderRepository.ExistsByOrderNumberAsync(orderNumber, cancellationToken))
            throw new InvalidOperationException(OrderErrorMessages.GeneratedOrderNumberAlreadyExists);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone,
            OrderType = request.OrderType,
            Priority = request.Priority,
            Channel = request.Channel,
            WarehouseId = request.WarehouseId,
            OrderDateUtc = DateTime.UtcNow,
            RequiredDateUtc = request.RequiredDateUtc,
            CurrencyCode = request.CurrencyCode,
            ReferenceNumber = request.ReferenceNumber,
            CustomerPurchaseOrderNumber = request.CustomerPurchaseOrderNumber,
            Notes = request.Notes,
            Status = OrderStatus.Draft,
            PaymentStatus = OrderPaymentStatus.Unpaid,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        decimal subtotal = 0m;
        decimal discount = 0m;
        decimal tax = 0m;

        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);
            if (product == null || product.IsDeleted)
                throw new KeyNotFoundException(OrderErrorMessages.ProductByIdNotFound(itemRequest.ProductId));

            if (itemRequest.QuantityOrdered <= 0)
                throw new InvalidOperationException(OrderErrorMessages.ItemQuantityMustBeGreaterThanZero);

            var lineSubtotal = itemRequest.QuantityOrdered * itemRequest.UnitPrice;
            var lineTotal = lineSubtotal - itemRequest.DiscountAmount + itemRequest.TaxAmount;

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                UnitOfMeasureId = itemRequest.UnitOfMeasureId,
                ProductNameSnapshot = product.Name,
                ProductSkuSnapshot = product.SKU,
                ProductDescriptionSnapshot = product.Description,
                QuantityOrdered = itemRequest.QuantityOrdered,
                QuantityAllocated = 0,
                QuantityShipped = 0,
                QuantityDelivered = 0,
                QuantityCancelled = 0,
                UnitPrice = itemRequest.UnitPrice,
                DiscountAmount = itemRequest.DiscountAmount,
                TaxAmount = itemRequest.TaxAmount,
                LineTotal = lineTotal,
                Remarks = itemRequest.Remarks,
                SortOrder = itemRequest.SortOrder,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            });

            subtotal += lineSubtotal;
            discount += itemRequest.DiscountAmount;
            tax += itemRequest.TaxAmount;
        }

        foreach (var address in request.Addresses ?? Enumerable.Empty<CreateOrderAddressRequest>())
        {
            order.Addresses.Add(new OrderAddress
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                AddressType = address.AddressType,
                ContactName = address.ContactName,
                CompanyName = address.CompanyName,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                StateOrProvince = address.StateOrProvince,
                PostalCode = address.PostalCode,
                Country = address.Country,
                PhoneNumber = address.PhoneNumber,
                Email = address.Email,
                // IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        order.SubtotalAmount = subtotal;
        order.DiscountAmount = discount;
        order.TaxAmount = tax;
        order.ShippingAmount = 0m;
        order.TotalAmount = subtotal - discount + tax;
        order.PaidAmount = 0m;
        order.RefundedAmount = 0m;
        order.OutstandingAmount = order.TotalAmount;

        order.StatusHistory.Add(new OrderStatusHistory
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            FromStatus = OrderStatus.Draft,
            ToStatus = OrderStatus.Draft,
            ChangedAtUtc = DateTime.UtcNow,
            ChangedBy = "System",
            Comments = "Order created.",
            // IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
            TotalAmount = order.TotalAmount,
            PaidAmount = order.PaidAmount,
            OutstandingAmount = order.OutstandingAmount,
            CurrencyCode = order.CurrencyCode
        };
    }

    public Task<BulkCreateResponse<OrderResponse>> CreateBulkAsync(
        BulkCreateRequest<CreateOrderRequest> request,
        CancellationToken cancellationToken = default) =>
        BulkCreateExecutor.ExecuteAsync(
            request.Items,
            (item, token) => CreateAsync(item, token),
            cancellationToken);

    public async Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.PendingApproval)
            throw new InvalidOperationException(OrderErrorMessages.OnlyDraftOrPendingOrdersCanBeUpdated);

        if (request.WarehouseId.HasValue)
        {
            var warehouseExists = await _warehouseRepository.ExistsAsync(x => x.Id == request.WarehouseId.Value && x.IsActive, cancellationToken);
            if (!warehouseExists)
                throw new KeyNotFoundException(OrderErrorMessages.WarehouseNotFound);
        }

        order.CustomerName = request.CustomerName;
        order.CustomerEmail = request.CustomerEmail;
        order.CustomerPhone = request.CustomerPhone;
        order.Priority = request.Priority;
        order.WarehouseId = request.WarehouseId;
        order.RequiredDateUtc = request.RequiredDateUtc;
        order.ReferenceNumber = request.ReferenceNumber;
        order.CustomerPurchaseOrderNumber = request.CustomerPurchaseOrderNumber;
        order.Notes = request.Notes;
        order.UpdatedAtUtc = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
            TotalAmount = order.TotalAmount,
            PaidAmount = order.PaidAmount,
            OutstandingAmount = order.OutstandingAmount,
            CurrencyCode = order.CurrencyCode
        };
    }

    public async Task<OrderDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (order == null) return null;

        return MapOrderDetail(order);
    }

    public async Task<OrderDetailResponse?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByOrderNumberWithDetailsAsync(orderNumber, cancellationToken);
        if (order == null) return null;

        return MapOrderDetail(order);
    }

    public async Task<PagedResponse<OrderListItemResponse>> GetPagedAsync(OrderQueryRequest request, CancellationToken cancellationToken = default)
    {
        var items = await _orderRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Status,
            request.OrderType,
            request.WarehouseId,
            cancellationToken);

        var count = await _orderRepository.CountAsync(
            request.SearchTerm,
            request.Status,
            request.OrderType,
            request.WarehouseId,
            cancellationToken);

        return new PagedResponse<OrderListItemResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = count,
            Items = items.Select(x => new OrderListItemResponse
            {
                Id = x.Id,
                OrderNumber = x.OrderNumber,
                CustomerName = x.CustomerName,
                OrderType = x.OrderType,
                Status = x.Status,
                PaymentStatus = x.PaymentStatus,
                TotalAmount = x.TotalAmount,
                OrderDateUtc = x.OrderDateUtc
            }).ToList()
        };
    }

    public async Task<OrderOverviewMetricsSummaryResponse> GetOverviewMetricsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var statuses = await _orderRepository.Query()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);

        return new OrderOverviewMetricsSummaryResponse
        {
            TotalOrders = statuses.Count,
            AwaitingAction = statuses.Count(status => status == OrderStatus.PendingApproval || status == OrderStatus.Approved),
            Processing = statuses.Count(status => status == OrderStatus.Processing),
            ShippedOrDelivered = statuses.Count(status => status == OrderStatus.Shipped || status == OrderStatus.Delivered)
        };
    }

    public async Task<OrderCustomerMetricsSummaryResponse> GetCustomerMetricsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.Query()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => new { x.CustomerName, x.TotalAmount })
            .ToListAsync(cancellationToken);

        var grouped = orders
            .GroupBy(order => string.IsNullOrWhiteSpace(order.CustomerName) ? "Walk-in / Internal" : order.CustomerName.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => new
            {
                OrderCount = group.Count(),
                TotalAmount = group.Sum(order => order.TotalAmount)
            })
            .ToList();

        var totalValue = grouped.Sum(group => group.TotalAmount);

        return new OrderCustomerMetricsSummaryResponse
        {
            TotalCustomers = grouped.Count,
            RepeatCustomers = grouped.Count(group => group.OrderCount > 1),
            TotalValue = totalValue,
            AverageCustomerValue = grouped.Count == 0 ? 0 : decimal.Round(totalValue / grouped.Count, 2)
        };
    }

    public async Task<OrderResponse> ChangeStatusAsync(Guid id, ChangeOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        ValidateStatusTransition(order.Status, request.NewStatus, request.Reason);

        var previousStatus = order.Status;
        order.Status = request.NewStatus;
        order.UpdatedAtUtc = DateTime.UtcNow;

        if (request.NewStatus == OrderStatus.Cancelled)
            order.CancelledDateUtc = DateTime.UtcNow;

        if (request.NewStatus == OrderStatus.Fulfilled)
            order.FulfilledDateUtc = DateTime.UtcNow;

        _orderRepository.Update(order);

        await _orderStatusHistoryRepository.AddAsync(new OrderStatusHistory
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            FromStatus = previousStatus,
            ToStatus = request.NewStatus,
            Reason = request.Reason,
            ChangedBy = request.ChangedBy,
            ChangedAtUtc = DateTime.UtcNow,
            Comments = request.Comments,
            // IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
            TotalAmount = order.TotalAmount,
            PaidAmount = order.PaidAmount,
            OutstandingAmount = order.OutstandingAmount,
            CurrencyCode = order.CurrencyCode
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        if (order.Status == OrderStatus.Fulfilled || order.Status == OrderStatus.Shipped)
            throw new InvalidOperationException(OrderErrorMessages.FulfilledOrShippedCannotBeDeleted);

        order.IsActive = false;
        order.UpdatedAtUtc = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus, string? reason)
    {
        if (currentStatus == newStatus)
            throw new InvalidOperationException(OrderErrorMessages.OrderAlreadyInRequestedStatus);

        if (newStatus == OrderStatus.Cancelled && string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException(OrderErrorMessages.CancellationReasonRequired);
    }

    private static OrderDetailResponse MapOrderDetail(Order order)
    {
        return new OrderDetailResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
            TotalAmount = order.TotalAmount,
            PaidAmount = order.PaidAmount,
            OutstandingAmount = order.OutstandingAmount,
            CurrencyCode = order.CurrencyCode,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            CustomerPhone = order.CustomerPhone,
            OrderType = order.OrderType,
            Priority = order.Priority,
            Channel = order.Channel,
            WarehouseId = order.WarehouseId,
            OrderDateUtc = order.OrderDateUtc,
            RequiredDateUtc = order.RequiredDateUtc,
            ReferenceNumber = order.ReferenceNumber,
            CustomerPurchaseOrderNumber = order.CustomerPurchaseOrderNumber,
            Notes = order.Notes,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                OrderId = i.OrderId,
                ProductId = i.ProductId,
                ProductNameSnapshot = i.ProductNameSnapshot,
                ProductSkuSnapshot = i.ProductSkuSnapshot,
                QuantityOrdered = i.QuantityOrdered,
                UnitPrice = i.UnitPrice,
                DiscountAmount = i.DiscountAmount,
                TaxAmount = i.TaxAmount,
                LineTotal = i.LineTotal
            }).ToList(),
            Addresses = order.Addresses.Select(a => new OrderAddressResponse
            {
                Id = a.Id,
                OrderId = a.OrderId,
                AddressType = a.AddressType,
                ContactName = a.ContactName,
                CompanyName = a.CompanyName,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                StateOrProvince = a.StateOrProvince,
                PostalCode = a.PostalCode,
                Country = a.Country
            }).ToList(),
            Images = order.Images.Select(i => new OrderImageResponse
            {
                Id = i.Id,
                OrderId = i.OrderId,
                FileName = i.FileName,
                PublicUrl = i.PublicUrl,
                ImageType = i.ImageType,
                Caption = i.Caption,
                IsPrimary = i.IsPrimary
            }).ToList(),
            NotesList = order.OrderNotes.Select(n => new OrderNoteResponse
            {
                Id = n.Id,
                OrderId = n.OrderId,
                Note = n.Note,
                IsInternal = n.IsInternal,
                CreatedBy = n.CreatedBy,
                CreatedAtUtc = n.CreatedAtUtc
            }).ToList(),
            StatusHistory = order.StatusHistory.Select(s => new OrderStatusHistoryResponse
            {
                Id = s.Id,
                OrderId = s.OrderId,
                FromStatus = s.FromStatus,
                ToStatus = s.ToStatus,
                Reason = s.Reason,
                ChangedBy = s.ChangedBy,
                ChangedAtUtc = s.ChangedAtUtc,
                Comments = s.Comments
            }).ToList(),
            Payments = order.Payments.Select(p => new OrderPaymentResponse
            {
                Id = p.Id,
                OrderId = p.OrderId,
                PaymentReference = p.PaymentReference,
                PaymentMethod = p.PaymentMethod,
                PaymentProvider = p.PaymentProvider,
                TransactionType = p.TransactionType,
                Status = p.Status,
                Amount = p.Amount,
                FeeAmount = p.FeeAmount,
                NetAmount = p.NetAmount,
                RefundedAmount = p.RefundedAmount,
                CurrencyCode = p.CurrencyCode,
                PaymentDateUtc = p.PaymentDateUtc
            }).ToList()
        };
    }
}
