namespace OperationIntelligence.Core;

public class ProductBulkCreateResponse
{
    public int TotalRequested { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public IReadOnlyList<ProductBulkCreateItemResult> Results { get; set; } = [];
}

public class ProductBulkCreateItemResult
{
    public int SourceRowNumber { get; set; }
    public string? ClientRowId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public ProductResponse? Product { get; set; }
}
