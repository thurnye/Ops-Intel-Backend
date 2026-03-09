using FluentValidation;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class DeliveryRunService : IDeliveryRunService
{
    private readonly IDeliveryRunRepository _deliveryRunRepository;
    private readonly IShipmentLookupRepository _lookupRepository;
    private readonly IValidator<CreateDeliveryRunRequest> _createValidator;
    private readonly IValidator<UpdateDeliveryRunRequest> _updateValidator;

    public DeliveryRunService(
        IDeliveryRunRepository deliveryRunRepository,
        IShipmentLookupRepository lookupRepository,
        IValidator<CreateDeliveryRunRequest> createValidator,
        IValidator<UpdateDeliveryRunRequest> updateValidator)
    {
        _deliveryRunRepository = deliveryRunRepository;
        _lookupRepository = lookupRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResponse<DeliveryRunResponse>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, DeliveryRunStatus? status = null, Guid? warehouseId = null, DateTime? plannedStartFromUtc = null, DateTime? plannedStartToUtc = null, CancellationToken cancellationToken = default)
    {
        var items = await _deliveryRunRepository.GetPagedAsync(pageNumber, pageSize, search, status, warehouseId, plannedStartFromUtc, plannedStartToUtc, cancellationToken);
        var total = await _deliveryRunRepository.CountAsync(search, status, warehouseId, plannedStartFromUtc, plannedStartToUtc, cancellationToken);

        return new PagedResponse<DeliveryRunResponse>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = total,
            Items = items.Select(Map).ToList()
        };
    }

    public async Task<DeliveryRunResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _deliveryRunRepository.GetByIdWithShipmentsAsync(id, cancellationToken);
        return entity == null ? null : Map(entity);
    }

    public async Task<DeliveryRunResponse> CreateAsync(CreateDeliveryRunRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await _lookupRepository.WarehouseExistsAsync(request.WarehouseId, cancellationToken))
            throw new KeyNotFoundException("Warehouse not found.");

        if (await _deliveryRunRepository.ExistsAsync(x => x.RunNumber == request.RunNumber, cancellationToken))
            throw new InvalidOperationException("Delivery run number already exists.");

        var entity = new DeliveryRun
        {
            RunNumber = request.RunNumber,
            Name = request.Name,
            WarehouseId = request.WarehouseId,
            PlannedStartUtc = request.PlannedStartUtc,
            PlannedEndUtc = request.PlannedEndUtc,
            DriverName = request.DriverName,
            VehicleNumber = request.VehicleNumber,
            RouteCode = request.RouteCode,
            Status = DeliveryRunStatus.Planned,
            Notes = request.Notes,
            CreatedBy = currentUser
        };

        await _deliveryRunRepository.AddAsync(entity, cancellationToken);
        await _deliveryRunRepository.SaveChangesAsync(cancellationToken);

        var created = await _deliveryRunRepository.GetByIdWithShipmentsAsync(entity.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created delivery run could not be reloaded.");

        return Map(created);
    }

    public async Task<DeliveryRunResponse> UpdateAsync(Guid id, UpdateDeliveryRunRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _deliveryRunRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery run not found.");

        entity.Name = request.Name;
        entity.PlannedStartUtc = request.PlannedStartUtc;
        entity.PlannedEndUtc = request.PlannedEndUtc;
        entity.ActualStartUtc = request.ActualStartUtc;
        entity.ActualEndUtc = request.ActualEndUtc;
        entity.DriverName = request.DriverName;
        entity.VehicleNumber = request.VehicleNumber;
        entity.RouteCode = request.RouteCode;
        entity.Status = request.Status;
        entity.Notes = request.Notes;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = currentUser;

        _deliveryRunRepository.Update(entity);
        await _deliveryRunRepository.SaveChangesAsync(cancellationToken);

        var updated = await _deliveryRunRepository.GetByIdWithShipmentsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Updated delivery run could not be reloaded.");

        return Map(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        var entity = await _deliveryRunRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;

        var assignedCount = await _deliveryRunRepository.CountAssignedShipmentsAsync(id, cancellationToken);
        if (assignedCount > 0)
            throw new InvalidOperationException("Cannot delete a delivery run with assigned shipments.");

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = currentUser;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = currentUser;

        _deliveryRunRepository.Update(entity);
        await _deliveryRunRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ShipmentListItemResponse>> GetAssignedShipmentsAsync(Guid deliveryRunId, CancellationToken cancellationToken = default)
    {
        if (!await _lookupRepository.DeliveryRunExistsAsync(deliveryRunId, cancellationToken))
            throw new KeyNotFoundException("Delivery run not found.");

        var items = await _deliveryRunRepository.GetAssignedShipmentsAsync(deliveryRunId, cancellationToken);
        return items.Select(x => new ShipmentListItemResponse
        {
            Id = x.Id,
            ShipmentNumber = x.ShipmentNumber,
            OrderId = x.OrderId,
            OrderNumber = x.Order?.OrderNumber,
            WarehouseName = x.Warehouse?.Name,
            CarrierName = x.Carrier?.Name,
            Type = x.Type,
            Status = x.Status,
            Priority = x.Priority,
            TrackingNumber = x.TrackingNumber,
            PlannedShipDateUtc = x.PlannedShipDateUtc,
            PlannedDeliveryDateUtc = x.PlannedDeliveryDateUtc,
            TotalWeight = x.TotalWeight,
            TotalPackages = x.TotalPackages,
            IsCrossBorder = x.IsCrossBorder,
            IsPartialShipment = x.IsPartialShipment,
            CreatedAtUtc = x.CreatedAtUtc
        }).ToList();
    }

    public async Task<IReadOnlyList<DeliveryRunResponse>> GetActiveRunsAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var items = await _deliveryRunRepository.GetActiveRunsAsync(warehouseId, cancellationToken);
        return items.Select(Map).ToList();
    }

    private static DeliveryRunResponse Map(DeliveryRun x) => new()
    {
        Id = x.Id,
        RunNumber = x.RunNumber,
        Name = x.Name,
        WarehouseId = x.WarehouseId,
        WarehouseName = x.Warehouse?.Name ?? string.Empty,
        PlannedStartUtc = x.PlannedStartUtc,
        PlannedEndUtc = x.PlannedEndUtc,
        ActualStartUtc = x.ActualStartUtc,
        ActualEndUtc = x.ActualEndUtc,
        DriverName = x.DriverName,
        VehicleNumber = x.VehicleNumber,
        RouteCode = x.RouteCode,
        Status = x.Status,
        Notes = x.Notes
    };
}