namespace OperationIntelligence.DB;

public enum ShipmentType
{
    Outbound = 1,
    Transfer = 2,
    Return = 3,
    DropShip = 4
}

public enum ShipmentStatus
{
    Draft = 1,
    AwaitingAllocation = 2,
    Allocated = 3,
    Picking = 4,
    Picked = 5,
    Packing = 6,
    Packed = 7,
    ReadyToDispatch = 8,
    Dispatched = 9,
    InTransit = 10,
    OutForDelivery = 11,
    Delivered = 12,
    DeliveryFailed = 13,
    Returned = 14,
    Cancelled = 15
}

public enum ShipmentPriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4
}

public enum ShipmentItemStatus
{
    Pending = 1,
    Allocated = 2,
    Picked = 3,
    Packed = 4,
    Shipped = 5,
    Delivered = 6,
    Returned = 7,
    Cancelled = 8
}

public enum PackageStatus
{
    Draft = 1,
    Packed = 2,
    LabelGenerated = 3,
    Dispatched = 4,
    InTransit = 5,
    Delivered = 6,
    Damaged = 7,
    Lost = 8
}

public enum ShipmentDocumentType
{
    PackingSlip = 1,
    BillOfLading = 2,
    ShippingLabel = 3,
    Invoice = 4,
    ProofOfDelivery = 5,
    CustomsDocument = 6,
    Other = 7
}

public enum ShipmentExceptionType
{
    Delay = 1,
    Damage = 2,
    Lost = 3,
    AddressIssue = 4,
    CustomerUnavailable = 5,
    CustomsHold = 6,
    Weather = 7,
    Other = 8
}

public enum ShipmentChargeType
{
    Freight = 1,
    FuelSurcharge = 2,
    Insurance = 3,
    Handling = 4,
    Customs = 5,
    Tax = 6,
    Other = 7
}

public enum DeliveryRunStatus
{
    Draft = 1,
    Planned = 2,
    Dispatched = 3,
    InProgress = 4,
    Completed = 5,
    Cancelled = 6
}

public enum DockAppointmentStatus
{
    Scheduled = 1,
    CheckedIn = 2,
    Loading = 3,
    Completed = 4,
    Missed = 5,
    Cancelled = 6
}

public enum InsuranceStatus
{
    Pending = 1,
    Active = 2,
    Expired = 3,
    Claimed = 4,
    Cancelled = 5
}

public enum ReturnShipmentStatus
{
    Requested = 1,
    Approved = 2,
    InTransit = 3,
    Received = 4,
    Inspected = 5,
    Restocked = 6,
    Rejected = 7,
    Closed = 8
}

public enum CustomsDocumentType
{
    CommercialInvoice = 1,
    CertificateOfOrigin = 2,
    CustomsDeclaration = 3,
    ExportDeclaration = 4,
    ImportPermit = 5,
    PackingList = 6,
    Other = 7
}