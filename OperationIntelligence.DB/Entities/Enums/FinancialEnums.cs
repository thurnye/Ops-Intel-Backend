namespace OperationIntelligence.DB;

public enum AccountType
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Revenue = 4,
    Expense = 5
}

public enum JournalEntryStatus
{
    Draft = 1,
    Approved = 2,
    Posted = 3,
    Reversed = 4,
    Cancelled = 5
}

public enum InvoiceStatus
{
    Draft = 1,
    Issued = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Overdue = 5,
    Cancelled = 6,
    WrittenOff = 7
}


public enum VendorBillStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    PartiallyPaid = 4,
    Paid = 5,
    Overdue = 6,
    Cancelled = 7
}

public enum FiscalPeriodStatus
{
    Open = 1,
    Closed = 2,
    Locked = 3
}

public enum FinanceSourceModule
{
    Manual = 1,
    Order = 2,
    Inventory = 3,
    Production = 4,
    Scheduling = 5,
    Shipment = 6,
    Adjustment = 7
}