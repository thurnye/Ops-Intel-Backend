namespace OperationIntelligence.DB;

public class Order : OrderBaseEntity
{
    public string OrderNumber { get; set; } = default!;

    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    public OrderType OrderType { get; set; } = OrderType.Sales;
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public OrderPriority Priority { get; set; } = OrderPriority.Normal;
    public OrderChannel Channel { get; set; } = OrderChannel.Internal;

    public Guid? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public DateTime OrderDateUtc { get; set; }
    public DateTime? RequiredDateUtc { get; set; }
    public DateTime? ApprovedDateUtc { get; set; }
    public DateTime? FulfilledDateUtc { get; set; }
    public DateTime? CancelledDateUtc { get; set; }

    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public decimal PaidAmount { get; set; }
    public decimal RefundedAmount { get; set; }
    public decimal OutstandingAmount { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    public string CurrencyCode { get; set; } = "CAD";

    public string? ReferenceNumber { get; set; }
    public string? CustomerPurchaseOrderNumber { get; set; }
    public string? SalesPerson { get; set; }
    public string? ApprovedBy { get; set; }
    public string? CancellationReason { get; set; }
    public string? Notes { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderImage> Images { get; set; } = new List<OrderImage>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public ICollection<OrderAddress> Addresses { get; set; } = new List<OrderAddress>();
    public ICollection<OrderNote> OrderNotes { get; set; } = new List<OrderNote>();
    public ICollection<OrderPayment> Payments { get; set; } = new List<OrderPayment>();

    public bool IsActive { get; set; } = true;
}
