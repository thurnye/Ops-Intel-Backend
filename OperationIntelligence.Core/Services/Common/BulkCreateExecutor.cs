namespace OperationIntelligence.Core;

internal static class BulkCreateExecutor
{
    public static async Task<BulkCreateResponse<TResponse>> ExecuteAsync<TPayload, TResponse>(
        IReadOnlyList<BulkCreateItemRequest<TPayload>> items,
        Func<TPayload, CancellationToken, Task<TResponse>> createAsync,
        CancellationToken cancellationToken = default)
        where TPayload : class
        where TResponse : class
    {
        var results = new List<BulkCreateItemResult<TResponse>>();

        foreach (var item in items)
        {
            try
            {
                var created = await createAsync(item.Payload, cancellationToken);
                results.Add(new BulkCreateItemResult<TResponse>
                {
                    SourceRowNumber = item.SourceRowNumber,
                    ClientRowId = item.ClientRowId,
                    Success = true,
                    Data = created
                });
            }
            catch (Exception ex)
            {
                results.Add(new BulkCreateItemResult<TResponse>
                {
                    SourceRowNumber = item.SourceRowNumber,
                    ClientRowId = item.ClientRowId,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        return new BulkCreateResponse<TResponse>
        {
            TotalRequested = items.Count,
            SuccessCount = results.Count(result => result.Success),
            FailureCount = results.Count(result => !result.Success),
            Results = results
        };
    }
}
