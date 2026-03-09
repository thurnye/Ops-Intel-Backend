using OperationIntelligence.Core.Models.Scheduling.Requests.Material;
using OperationIntelligence.Core.Models.Scheduling.Responses.Material;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ScheduleMaterialService : IScheduleMaterialService
{
    private readonly IScheduleMaterialCheckRepository _scheduleMaterialCheckRepository;

    public ScheduleMaterialService(IScheduleMaterialCheckRepository scheduleMaterialCheckRepository)
    {
        _scheduleMaterialCheckRepository = scheduleMaterialCheckRepository;
    }

    public async Task<ScheduleMaterialCheckResponse> CreateAsync(CreateScheduleMaterialCheckRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ScheduleMaterialCheck
        {
            ScheduleJobId = request.ScheduleJobId,
            ScheduleOperationId = request.ScheduleOperationId,
            ProductionOrderId = request.ProductionOrderId,
            RoutingStepId = request.RoutingStepId,
            MaterialProductId = request.MaterialProductId,
            WarehouseId = request.WarehouseId,
            RequiredQuantity = request.RequiredQuantity,
            AvailableQuantity = request.AvailableQuantity,
            ReservedQuantity = request.ReservedQuantity,
            ShortageQuantity = request.ShortageQuantity,
            Status = (MaterialReadinessStatus)request.Status,
            ExpectedAvailabilityDateUtc = request.ExpectedAvailabilityDateUtc,
            Notes = request.Notes?.Trim(),
            CheckedAtUtc = request.CheckedAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _scheduleMaterialCheckRepository.AddAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleMaterialCheckResponse> UpdateAsync(Guid id, UpdateScheduleMaterialCheckRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleMaterialCheckRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleMaterialCheckNotFound);

        entity.RequiredQuantity = request.RequiredQuantity;
        entity.AvailableQuantity = request.AvailableQuantity;
        entity.ReservedQuantity = request.ReservedQuantity;
        entity.ShortageQuantity = request.ShortageQuantity;
        entity.Status = (MaterialReadinessStatus)request.Status;
        entity.ExpectedAvailabilityDateUtc = request.ExpectedAvailabilityDateUtc;
        entity.Notes = request.Notes?.Trim();
        entity.CheckedAtUtc = request.CheckedAtUtc;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleMaterialCheckRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleMaterialCheckResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleMaterialCheckRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleMaterialCheckRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _scheduleMaterialCheckRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private static ScheduleMaterialCheckResponse MapToResponse(ScheduleMaterialCheck entity)
    {
        return new ScheduleMaterialCheckResponse
        {
            Id = entity.Id,
            ScheduleJobId = entity.ScheduleJobId,
            ScheduleOperationId = entity.ScheduleOperationId,
            ProductionOrderId = entity.ProductionOrderId,
            RoutingStepId = entity.RoutingStepId,
            MaterialProductId = entity.MaterialProductId,
            MaterialProductName = entity.MaterialProduct?.Name ?? string.Empty,
            WarehouseId = entity.WarehouseId,
            WarehouseName = entity.Warehouse?.Name ?? string.Empty,
            RequiredQuantity = entity.RequiredQuantity,
            AvailableQuantity = entity.AvailableQuantity,
            ReservedQuantity = entity.ReservedQuantity,
            ShortageQuantity = entity.ShortageQuantity,
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            ExpectedAvailabilityDateUtc = entity.ExpectedAvailabilityDateUtc,
            Notes = entity.Notes,
            CheckedAtUtc = entity.CheckedAtUtc
        };
    }
}
