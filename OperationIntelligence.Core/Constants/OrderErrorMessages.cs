namespace OperationIntelligence.Core;

public static class OrderErrorMessages
{
    public const string OrderNotFound = "Order not found.";
    public const string WarehouseNotFound = "Warehouse not found.";
    public const string PaymentNotFound = "Payment not found.";
    public const string ImageNotFound = "Image not found.";
    public const string AddressNotFound = "Address not found.";
    public const string OrderItemNotFound = "Order item not found.";
    public const string ProductNotFound = "Product not found.";

    public const string BillingAddressAlreadyExists = "Billing address already exists for this order.";
    public const string ShippingAddressAlreadyExists = "Shipping address already exists for this order.";

    public const string InvalidFileSize = "Invalid file size.";
    public const string UnsupportedFileType = "Unsupported file type.";
    public const string NoteIsRequired = "Note is required.";

    public const string OrderMustContainAtLeastOneItem = "An order must contain at least one item.";
    public const string GeneratedOrderNumberAlreadyExists = "Generated order number already exists.";
    public const string ItemQuantityMustBeGreaterThanZero = "Item quantity must be greater than zero.";
    public const string OrderAlreadyInRequestedStatus = "Order is already in the requested status.";
    public const string CancellationReasonRequired = "Cancellation reason is required.";
    public const string FulfilledOrShippedCannotBeDeleted = "Fulfilled or shipped orders cannot be deleted.";

    public const string ItemsCanOnlyBeAddedToDraftOrPending = "Items can only be added to draft or pending approval orders.";
    public const string ItemsCanOnlyBeUpdatedOnDraftOrPending = "Items can only be updated on draft or pending approval orders.";
    public const string ItemsCanOnlyBeRemovedFromDraftOrPending = "Items can only be removed from draft or pending approval orders.";
    public const string OnlyDraftOrPendingOrdersCanBeUpdated = "Only draft or pending approval orders can be updated.";

    public const string PaymentAmountMustBeGreaterThanZero = "Payment amount must be greater than zero.";
    public const string PaymentCurrencyMustMatchOrder = "Payment currency must match order currency.";
    public const string GeneratedPaymentReferenceAlreadyExists = "Generated payment reference already exists.";
    public const string InvalidRefundAmount = "Invalid refund amount.";

    public static string ProductByIdNotFound(Guid productId) => $"Product {productId} not found.";
}
