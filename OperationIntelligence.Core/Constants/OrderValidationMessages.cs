namespace OperationIntelligence.Core;

public static class OrderValidationMessages
{
    public const string AtLeastOneOrderItemRequired = "At least one order item is required.";
    public const string RequiredDateCannotBePast = "RequiredDateUtc cannot be in the past.";
    public const string CancellationReasonRequired = "Reason is required when cancelling an order.";
    public const string UnsupportedFileType = "Unsupported file type.";
}
