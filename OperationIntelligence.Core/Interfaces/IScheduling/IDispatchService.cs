using OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;
using OperationIntelligence.Core.Models.Scheduling.Responses.Dispatch;

namespace OperationIntelligence.Core;

public interface IDispatchService
{
    Task<DispatchQueueItemResponse> CreateQueueItemAsync(
        CreateDispatchQueueItemRequest request,
        CancellationToken cancellationToken = default);

    Task<DispatchQueueItemResponse> ReleaseAsync(
        Guid id,
        ReleaseDispatchQueueItemRequest request,
        CancellationToken cancellationToken = default);

    Task<DispatchQueueItemResponse> AcknowledgeAsync(
        Guid id,
        AcknowledgeDispatchQueueItemRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DispatchQueueItemResponse>> ResequenceAsync(
        ResequenceDispatchQueueRequest request,
        CancellationToken cancellationToken = default);

    Task<DispatchQueueItemResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
