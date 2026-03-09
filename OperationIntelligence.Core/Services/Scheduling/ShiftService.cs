using Microsoft.Extensions.Logging;
using OperationIntelligence.Core.Models.Scheduling.Requests.Shift;
using OperationIntelligence.Core.Models.Scheduling.Responses.Shift;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShiftService : IShiftService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly ILogger<ShiftService> _logger;

    public ShiftService(IShiftRepository shiftRepository, ILogger<ShiftService> logger)
    {
        _shiftRepository = shiftRepository;
        _logger = logger;
    }

    public async Task<ShiftResponse> CreateAsync(CreateShiftRequest request, CancellationToken cancellationToken = default)
    {
        if (await _shiftRepository.ExistsByCodeAsync(request.WarehouseId, request.ShiftCode.Trim(), null, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.ShiftCodeAlreadyExistsInWarehouse);

        var entity = new Shift
        {
            WarehouseId = request.WarehouseId,
            WorkCenterId = request.WorkCenterId,
            ShiftCode = request.ShiftCode.Trim(),
            ShiftName = request.ShiftName.Trim(),
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            CrossesMidnight = request.CrossesMidnight,
            IsActive = true,
            CapacityMinutes = request.CapacityMinutes,
            BreakMinutes = request.BreakMinutes,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _shiftRepository.AddAsync(entity, cancellationToken);
        var created = await _shiftRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;
        return MapToResponse(created);
    }

    public async Task<ShiftResponse> UpdateAsync(Guid id, UpdateShiftRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _shiftRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ShiftNotFound);

        entity.ShiftName = request.ShiftName.Trim();
        entity.StartTime = request.StartTime;
        entity.EndTime = request.EndTime;
        entity.CrossesMidnight = request.CrossesMidnight;
        entity.IsActive = request.IsActive;
        entity.CapacityMinutes = request.CapacityMinutes;
        entity.BreakMinutes = request.BreakMinutes;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _shiftRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ShiftResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _shiftRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<PagedResponse<ShiftResponse>> GetAllAsync(GetShiftsRequest request, CancellationToken cancellationToken = default)
    {
        var (items, totalRecords) = await _shiftRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        return new PagedResponse<ShiftResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            Items = items.Select(MapToResponse).ToList()
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _shiftRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _shiftRepository.DeleteAsync(entity, cancellationToken);
        _logger.LogInformation("Deleted shift {ShiftId}", id);
        return true;
    }

    private static ShiftResponse MapToResponse(Shift entity)
    {
        return new ShiftResponse
        {
            Id = entity.Id,
            WarehouseId = entity.WarehouseId,
            WarehouseName = entity.Warehouse?.Name ?? string.Empty,
            WorkCenterId = entity.WorkCenterId,
            WorkCenterName = entity.WorkCenter?.Name,
            ShiftCode = entity.ShiftCode,
            ShiftName = entity.ShiftName,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            CrossesMidnight = entity.CrossesMidnight,
            IsActive = entity.IsActive,
            CapacityMinutes = entity.CapacityMinutes,
            BreakMinutes = entity.BreakMinutes,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc ?? entity.CreatedAtUtc
        };
    }
}
