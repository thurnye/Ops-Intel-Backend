public enum OrderType
{
    Sales = 1,
    Purchase = 2,
    Transfer = 3,
    Return = 4
}

public enum OrderStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Processing = 4,
    PartiallyFulfilled = 5,
    Fulfilled = 6,
    Shipped = 7,
    Delivered = 8,
    Cancelled = 9,
    Rejected = 10,
    Returned = 11
}

public enum OrderPriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4
}

public enum OrderChannel
{
    Internal = 1,
    Web = 2,
    Mobile = 3,
    Phone = 4,
    Email = 5,
    Marketplace = 6
}

public enum OrderImageType
{
    General = 1,
    PackingSlip = 2,
    Invoice = 3,
    ProductPhoto = 4,
    DamageEvidence = 5,
    DeliveryProof = 6,
    SignatureProof = 7,
    ShippingLabel = 8
}

public enum AddressType
{
    Billing = 1,
    Shipping = 2
}
public enum PaymentStatus
{
    Unpaid = 1,
    Pending = 2,
    PartiallyPaid = 3,
    Paid = 4,
    PartiallyRefunded = 5,
    Refunded = 6,
    Failed = 7,
    Cancelled = 8
}

public enum PaymentMethod
{
    Cash = 1,
    Card = 2,
    BankTransfer = 3,
    MobileMoney = 4,
    Cheque = 5,
    Wallet = 6,
    Other = 7
}

public enum PaymentProvider
{
    Manual = 1,
    Stripe = 2,
    PayPal = 3,
    Square = 4,
    Moneris = 5,
    Flutterwave = 6,
    Paystack = 7,
    Other = 8
}

public enum PaymentTransactionType
{
    Payment = 1,
    Refund = 2,
    Adjustment = 3,
    Chargeback = 4
}