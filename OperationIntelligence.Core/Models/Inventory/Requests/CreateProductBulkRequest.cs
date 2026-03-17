namespace OperationIntelligence.Core;

public class CreateProductBulkRequest
{
    public IReadOnlyList<CreateProductBulkItemRequest> Items { get; set; } = [];
}

public class CreateProductBulkItemRequest : CreateProductRequest
{
    public int SourceRowNumber { get; set; }
    public string? ClientRowId { get; set; }
}
