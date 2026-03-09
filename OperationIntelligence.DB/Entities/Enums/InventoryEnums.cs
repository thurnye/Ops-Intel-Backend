namespace OperationIntelligence.DB;

public enum ProductStatus
{
    Draft = 1,
    Active = 2,
    Inactive = 3,
    Discontinued = 4
}


public enum StockMovementType
{
    StockIn = 1,
    StockOut = 2,
    AdjustmentIncrease = 3,
    AdjustmentDecrease = 4,
    TransferIn = 5,
    TransferOut = 6,
    ReturnIn = 7,
    ReturnOut = 8,
    Damaged = 9,
    Expired = 10
}