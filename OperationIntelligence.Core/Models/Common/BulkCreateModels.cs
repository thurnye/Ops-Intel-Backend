namespace OperationIntelligence.Core;

public class BulkCreateRequest<TPayload>
    where TPayload : class
{
    public IReadOnlyList<BulkCreateItemRequest<TPayload>> Items { get; set; } = [];
}

public class BulkCreateItemRequest<TPayload>
    where TPayload : class
{
    public int SourceRowNumber { get; set; }
    public string? ClientRowId { get; set; }
    public TPayload Payload { get; set; } = default!;
}

public class BulkCreateResponse<TResponse>
    where TResponse : class
{
    public int TotalRequested { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public IReadOnlyList<BulkCreateItemResult<TResponse>> Results { get; set; } = [];
}

public class BulkCreateItemResult<TResponse>
    where TResponse : class
{
    public int SourceRowNumber { get; set; }
    public string? ClientRowId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public TResponse? Data { get; set; }
}
