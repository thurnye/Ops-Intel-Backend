using FluentValidation;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class DockAppointmentService : IDockAppointmentService
{
    private readonly IDockAppointmentRepository _dockAppointmentRepository;
    private readonly IShipmentLookupRepository _lookupRepository;
    private readonly IValidator<CreateDockAppointmentRequest> _createValidator;
    private readonly IValidator<UpdateDockAppointmentRequest> _updateValidator;

    public DockAppointmentService(
        IDockAppointmentRepository dockAppointmentRepository,
        IShipmentLookupRepository lookupRepository,
        IValidator<CreateDockAppointmentRequest> createValidator,
        IValidator<UpdateDockAppointmentRequest> updateValidator)
    {
        _dockAppointmentRepository = dockAppointmentRepository;
        _lookupRepository = lookupRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResponse<DockAppointmentResponse>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, DockAppointmentStatus? status = null, Guid? warehouseId = null, Guid? carrierId = null, DateTime? scheduledStartFromUtc = null, DateTime? scheduledStartToUtc = null, CancellationToken cancellationToken = default)
    {
        var items = await _dockAppointmentRepository.GetPagedAsync(pageNumber, pageSize, search, status, warehouseId, carrierId, scheduledStartFromUtc, scheduledStartToUtc, cancellationToken);
        var total = await _dockAppointmentRepository.CountAsync(search, status, warehouseId, carrierId, scheduledStartFromUtc, scheduledStartToUtc, cancellationToken);

        return new PagedResponse<DockAppointmentResponse>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = total,
            Items = items.Select(Map).ToList()
        };
    }

    public async Task<DockAppointmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dockAppointmentRepository.GetByIdWithShipmentsAsync(id, cancellationToken);
        return entity == null ? null : Map(entity);
    }

    public async Task<DockAppointmentResponse> CreateAsync(CreateDockAppointmentRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await _lookupRepository.WarehouseExistsAsync(request.WarehouseId, cancellationToken))
            throw new KeyNotFoundException("Warehouse not found.");

        if (request.CarrierId.HasValue && !await _lookupRepository.CarrierExistsAsync(request.CarrierId.Value, cancellationToken))
            throw new KeyNotFoundException("Carrier not found.");

        if (await _dockAppointmentRepository.ExistsAsync(x => x.AppointmentNumber == request.AppointmentNumber, cancellationToken))
            throw new InvalidOperationException("Dock appointment number already exists.");

        if (!string.IsNullOrWhiteSpace(request.DockCode))
        {
            var conflict = await _dockAppointmentRepository.HasDockConflictAsync(
                request.WarehouseId,
                request.DockCode,
                request.ScheduledStartUtc,
                request.ScheduledEndUtc,
                null,
                cancellationToken);

            if (conflict)
                throw new InvalidOperationException("Dock conflict detected for the selected time window.");
        }

        var entity = new DockAppointment
        {
            AppointmentNumber = request.AppointmentNumber,
            WarehouseId = request.WarehouseId,
            CarrierId = request.CarrierId,
            DockCode = request.DockCode,
            TrailerNumber = request.TrailerNumber,
            DriverName = request.DriverName,
            ScheduledStartUtc = request.ScheduledStartUtc,
            ScheduledEndUtc = request.ScheduledEndUtc,
            Status = DockAppointmentStatus.Scheduled,
            Notes = request.Notes,
            CreatedBy = currentUser
        };

        await _dockAppointmentRepository.AddAsync(entity, cancellationToken);
        await _dockAppointmentRepository.SaveChangesAsync(cancellationToken);

        var created = await _dockAppointmentRepository.GetByIdWithShipmentsAsync(entity.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created dock appointment could not be reloaded.");

        return Map(created);
    }

    public async Task<DockAppointmentResponse> UpdateAsync(Guid id, UpdateDockAppointmentRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _dockAppointmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Dock appointment not found.");

        if (request.CarrierId.HasValue && !await _lookupRepository.CarrierExistsAsync(request.CarrierId.Value, cancellationToken))
            throw new KeyNotFoundException("Carrier not found.");

        if (!string.IsNullOrWhiteSpace(request.DockCode))
        {
            var conflict = await _dockAppointmentRepository.HasDockConflictAsync(
                entity.WarehouseId,
                request.DockCode,
                request.ScheduledStartUtc,
                request.ScheduledEndUtc,
                entity.Id,
                cancellationToken);

            if (conflict)
                throw new InvalidOperationException("Dock conflict detected for the selected time window.");
        }

        entity.CarrierId = request.CarrierId;
        entity.DockCode = request.DockCode;
        entity.TrailerNumber = request.TrailerNumber;
        entity.DriverName = request.DriverName;
        entity.ScheduledStartUtc = request.ScheduledStartUtc;
        entity.ScheduledEndUtc = request.ScheduledEndUtc;
        entity.ActualArrivalUtc = request.ActualArrivalUtc;
        entity.ActualDepartureUtc = request.ActualDepartureUtc;
        entity.Status = request.Status;
        entity.Notes = request.Notes;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = currentUser;

        _dockAppointmentRepository.Update(entity);
        await _dockAppointmentRepository.SaveChangesAsync(cancellationToken);

        var updated = await _dockAppointmentRepository.GetByIdWithShipmentsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Updated dock appointment could not be reloaded.");

        return Map(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        var entity = await _dockAppointmentRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = currentUser;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = currentUser;

        _dockAppointmentRepository.Update(entity);
        await _dockAppointmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<DockAppointmentResponse>> GetUpcomingAsync(Guid? warehouseId = null, DateTime? fromUtc = null, CancellationToken cancellationToken = default)
    {
        var items = await _dockAppointmentRepository.GetUpcomingAsync(warehouseId, fromUtc, cancellationToken);
        return items.Select(Map).ToList();
    }

    private static DockAppointmentResponse Map(DockAppointment x) => new()
    {
        Id = x.Id,
        AppointmentNumber = x.AppointmentNumber,
        WarehouseId = x.WarehouseId,
        WarehouseName = x.Warehouse?.Name ?? string.Empty,
        CarrierId = x.CarrierId,
        CarrierName = x.Carrier?.Name,
        DockCode = x.DockCode,
        TrailerNumber = x.TrailerNumber,
        DriverName = x.DriverName,
        ScheduledStartUtc = x.ScheduledStartUtc,
        ScheduledEndUtc = x.ScheduledEndUtc,
        ActualArrivalUtc = x.ActualArrivalUtc,
        ActualDepartureUtc = x.ActualDepartureUtc,
        Status = x.Status,
        Notes = x.Notes
    };
}