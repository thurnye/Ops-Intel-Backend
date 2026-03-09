
using OperationIntelligence.Core.Models.Scheduling.Requests.Capacity;
using OperationIntelligence.Core.Models.Scheduling.Responses.Capacity;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class CapacityService : ICapacityService
{
    private readonly ICapacityReservationRepository _capacityReservationRepository;
    private readonly IResourceCapacitySnapshotRepository _resourceCapacitySnapshotRepository;

    public CapacityService(
        ICapacityReservationRepository capacityReservationRepository,
        IResourceCapacitySnapshotRepository resourceCapacitySnapshotRepository)
    {
        _capacityReservationRepository = capacityReservationRepository;
        _resourceCapacitySnapshotRepository = resourceCapacitySnapshotRepository;
    }

    public async Task<CapacityReservationResponse> CreateReservationAsync(CreateCapacityReservationRequest request, CancellationToken cancellationToken = default)
    {
        if (await _capacityReservationRepository.HasOverlapAsync(request.ResourceId, (ResourceType)request.ResourceType, request.ReservedStartUtc, request.ReservedEndUtc, null, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingCapacityReservationDetected);

        var entity = new CapacityReservation
        {
            ScheduleOperationId = request.ScheduleOperationId,
            ResourceId = request.ResourceId,
            ResourceType = (ResourceType)request.ResourceType,
            ShiftId = request.ShiftId,
            ReservedStartUtc = request.ReservedStartUtc,
            ReservedEndUtc = request.ReservedEndUtc,
            ReservedMinutes = request.ReservedMinutes,
            AvailableMinutesAtBooking = request.AvailableMinutesAtBooking,
            Status = CapacityReservationStatus.Reserved,
            ReservationReason = request.ReservationReason.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _capacityReservationRepository.AddAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<CapacityReservationResponse> UpdateReservationAsync(Guid id, UpdateCapacityReservationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _capacityReservationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.CapacityReservationNotFound);

        if (await _capacityReservationRepository.HasOverlapAsync(entity.ResourceId, entity.ResourceType, request.ReservedStartUtc, request.ReservedEndUtc, id, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingCapacityReservationDetected);

        entity.ShiftId = request.ShiftId;
        entity.ReservedStartUtc = request.ReservedStartUtc;
        entity.ReservedEndUtc = request.ReservedEndUtc;
        entity.ReservedMinutes = request.ReservedMinutes;
        entity.AvailableMinutesAtBooking = request.AvailableMinutesAtBooking;
        entity.Status = (CapacityReservationStatus)request.Status;
        entity.ReservationReason = request.ReservationReason.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _capacityReservationRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<CapacityReservationResponse?> GetReservationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _capacityReservationRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<bool> DeleteReservationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _capacityReservationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _capacityReservationRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    public async Task<PagedResponse<ResourceCapacitySnapshotResponse>> GetUtilizationAsync(GetCapacityUtilizationRequest request, CancellationToken cancellationToken = default)
    {
        var (items, totalRecords) = await _resourceCapacitySnapshotRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        return new PagedResponse<ResourceCapacitySnapshotResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            Items = items.Select(x => new ResourceCapacitySnapshotResponse
            {
                Id = x.Id,
                ResourceId = x.ResourceId,
                ResourceType = (int)x.ResourceType,
                ResourceTypeName = x.ResourceType.ToString(),
                SnapshotDateUtc = x.SnapshotDateUtc,
                ShiftId = x.ShiftId,
                ShiftName = x.Shift?.ShiftName,
                TotalCapacityMinutes = x.TotalCapacityMinutes,
                ReservedMinutes = x.ReservedMinutes,
                AvailableMinutes = x.AvailableMinutes,
                OvertimeMinutes = x.OvertimeMinutes,
                IsOverloaded = x.IsOverloaded,
                IsBottleneck = x.IsBottleneck
            }).ToList()
        };
    }

    private static CapacityReservationResponse MapToResponse(CapacityReservation entity)
    {
        return new CapacityReservationResponse
        {
            Id = entity.Id,
            ScheduleOperationId = entity.ScheduleOperationId,
            ResourceId = entity.ResourceId,
            ResourceType = (int)entity.ResourceType,
            ResourceTypeName = entity.ResourceType.ToString(),
            ShiftId = entity.ShiftId,
            ShiftName = entity.Shift?.ShiftName,
            ReservedStartUtc = entity.ReservedStartUtc,
            ReservedEndUtc = entity.ReservedEndUtc,
            ReservedMinutes = entity.ReservedMinutes,
            AvailableMinutesAtBooking = entity.AvailableMinutesAtBooking,
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            ReservationReason = entity.ReservationReason
        };
    }
}
