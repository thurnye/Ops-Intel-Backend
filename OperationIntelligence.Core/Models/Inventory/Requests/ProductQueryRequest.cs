using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductQueryRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public ProductStatus? Status { get; set; }
}
