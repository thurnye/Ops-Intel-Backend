
using OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;
using OperationIntelligence.Core.Models.Scheduling.Responses.Dispatch;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class DispatchService : IDispatchService
{
    private readonly IDispatchQueueRepository _dispatchQueueRepository;

    public DispatchService(IDispatchQueueRepository dispatchQueueRepository)
    {
        _dispatchQueueRepository = dispatchQueueRepository;
    }

    public async Task<DispatchQueueItemResponse> CreateQueueItemAsync(CreateDispatchQueueItemRequest request, CancellationToken cancellationToken = default)
    {
        if (await _dispatchQueueRepository.ExistsQueuePositionAsync(request.WorkCenterId, request.MachineId, request.QueuePosition, null, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.QueuePositionAlreadyExists);

        var entity = new DispatchQueueItem
        {
            ScheduleOperationId = request.ScheduleOperationId,
            WorkCenterId = request.WorkCenterId,
            MachineId = request.MachineId,
            QueuePosition = request.QueuePosition,
            PriorityScore = request.PriorityScore,
            Status = DispatchStatus.NotDispatched,
            DispatchNotes = request.DispatchNotes?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _dispatchQueueRepository.AddAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<DispatchQueueItemResponse> ReleaseAsync(Guid id, ReleaseDispatchQueueItemRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _dispatchQueueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.DispatchQueueItemNotFound);

        entity.Status = DispatchStatus.Dispatched;
        entity.ReleasedAtUtc = request.ReleasedAtUtc;
        entity.DispatchNotes = request.DispatchNotes?.Trim() ?? entity.DispatchNotes;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _dispatchQueueRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<DispatchQueueItemResponse> AcknowledgeAsync(Guid id, AcknowledgeDispatchQueueItemRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _dispatchQueueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.DispatchQueueItemNotFound);

        entity.Status = DispatchStatus.Acknowledged;
        entity.AcknowledgedAtUtc = request.AcknowledgedAtUtc;
        entity.DispatchNotes = request.DispatchNotes?.Trim() ?? entity.DispatchNotes;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _dispatchQueueRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<IReadOnlyList<DispatchQueueItemResponse>> ResequenceAsync(ResequenceDispatchQueueRequest request, CancellationToken cancellationToken = default)
    {
        var queue = await _dispatchQueueRepository.GetByWorkCenterAsync(request.WorkCenterId, request.MachineId, true, cancellationToken);
        var queueMap = queue.ToDictionary(x => x.Id, x => x);

        foreach (var item in request.Items)
        {
            if (!queueMap.TryGetValue(item.DispatchQueueItemId, out var entity))
                continue;

            entity.QueuePosition = item.QueuePosition;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dispatchQueueRepository.UpdateAsync(entity, cancellationToken);
        }

        var updated = await _dispatchQueueRepository.GetByWorkCenterAsync(request.WorkCenterId, request.MachineId, true, cancellationToken);
        return updated.Select(MapToResponse).ToList();
    }

    public async Task<DispatchQueueItemResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dispatchQueueRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dispatchQueueRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _dispatchQueueRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private static DispatchQueueItemResponse MapToResponse(DispatchQueueItem entity)
    {
        return new DispatchQueueItemResponse
        {
            Id = entity.Id,
            ScheduleOperationId = entity.ScheduleOperationId,
            WorkCenterId = entity.WorkCenterId,
            WorkCenterName = entity.WorkCenter?.Name ?? string.Empty,
            MachineId = entity.MachineId,
            MachineName = entity.Machine?.Name,
            QueuePosition = entity.QueuePosition,
            PriorityScore = entity.PriorityScore,
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            ReleasedAtUtc = entity.ReleasedAtUtc,
            AcknowledgedAtUtc = entity.AcknowledgedAtUtc,
            DispatchNotes = entity.DispatchNotes,
            IsActive = entity.IsActive
        };
    }
}
