using FluentValidation;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ReturnShipmentService : IReturnShipmentService
{
    private readonly IReturnShipmentRepository _returnShipmentRepository;
    private readonly IShipmentLookupRepository _lookupRepository;
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IValidator<CreateReturnShipmentRequest> _createValidator;
    private readonly IValidator<UpdateReturnShipmentRequest> _updateValidator;
    private readonly IValidator<AddReturnShipmentItemRequest> _addItemValidator;
    private readonly IValidator<ReturnShipmentFilterRequest> _filterValidator;

    public ReturnShipmentService(
        IReturnShipmentRepository returnShipmentRepository,
        IShipmentLookupRepository lookupRepository,
        IShipmentRepository shipmentRepository,
        IValidator<CreateReturnShipmentRequest> createValidator,
        IValidator<UpdateReturnShipmentRequest> updateValidator,
        IValidator<AddReturnShipmentItemRequest> addItemValidator,
        IValidator<ReturnShipmentFilterRequest> filterValidator)
    {
        _returnShipmentRepository = returnShipmentRepository;
        _lookupRepository = lookupRepository;
        _shipmentRepository = shipmentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _addItemValidator = addItemValidator;
        _filterValidator = filterValidator;
    }

    public async Task<PagedResponse<ReturnShipmentResponse>> GetPagedAsync(ReturnShipmentFilterRequest request, CancellationToken cancellationToken = default)
    {
        await _filterValidator.ValidateAndThrowAsync(request, cancellationToken);

        var items = await _returnShipmentRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.Status,
            request.ShipmentId,
            request.OrderId,
            request.CarrierId,
            request.RequestedFromUtc,
            request.RequestedToUtc,
            cancellationToken);

        var total = await _returnShipmentRepository.CountAsync(
            request.Search,
            request.Status,
            request.ShipmentId,
            request.OrderId,
            request.CarrierId,
            request.RequestedFromUtc,
            request.RequestedToUtc,
            cancellationToken);

        return new PagedResponse<ReturnShipmentResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = total,
            Items = items.Select(Map).ToList()
        };
    }

    public async Task<ReturnShipmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _returnShipmentRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return entity == null ? null : Map(entity);
    }

    public async Task<ReturnShipmentResponse> CreateAsync(CreateReturnShipmentRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await _lookupRepository.ShipmentExistsAsync(request.ShipmentId, cancellationToken))
            throw new KeyNotFoundException("Shipment not found.");

        if (request.OrderId.HasValue && !await _lookupRepository.OrderExistsAsync(request.OrderId.Value, cancellationToken))
            throw new KeyNotFoundException("Order not found.");

        if (!await _lookupRepository.ShipmentAddressExistsAsync(request.OriginAddressId, cancellationToken))
            throw new KeyNotFoundException("Origin address not found.");

        if (!await _lookupRepository.ShipmentAddressExistsAsync(request.DestinationAddressId, cancellationToken))
            throw new KeyNotFoundException("Destination address not found.");

        if (request.CarrierId.HasValue && !await _lookupRepository.CarrierExistsAsync(request.CarrierId.Value, cancellationToken))
            throw new KeyNotFoundException("Carrier not found.");

        if (request.CarrierServiceId.HasValue && !await _lookupRepository.CarrierServiceExistsAsync(request.CarrierServiceId.Value, cancellationToken))
            throw new KeyNotFoundException("Carrier service not found.");

        if (await _returnShipmentRepository.ExistsAsync(x => x.ReturnShipmentNumber == request.ReturnShipmentNumber, cancellationToken))
            throw new InvalidOperationException("Return shipment number already exists.");

        var entity = new ReturnShipment
        {
            ReturnShipmentNumber = request.ReturnShipmentNumber,
            ShipmentId = request.ShipmentId,
            OrderId = request.OrderId,
            OriginAddressId = request.OriginAddressId,
            DestinationAddressId = request.DestinationAddressId,
            CarrierId = request.CarrierId,
            CarrierServiceId = request.CarrierServiceId,
            TrackingNumber = request.TrackingNumber,
            ReasonCode = request.ReasonCode,
            ReasonDescription = request.ReasonDescription,
            Status = ReturnShipmentStatus.Requested,
            RequestedAtUtc = request.RequestedAtUtc,
            Notes = request.Notes,
            CreatedBy = currentUser
        };

        await _returnShipmentRepository.AddAsync(entity, cancellationToken);
        await _returnShipmentRepository.SaveChangesAsync(cancellationToken);

        var created = await _returnShipmentRepository.GetByIdWithDetailsAsync(entity.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created return shipment could not be reloaded.");

        return Map(created);
    }

    public async Task<ReturnShipmentResponse> UpdateAsync(Guid id, UpdateReturnShipmentRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _returnShipmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Return shipment not found.");

        if (!await _lookupRepository.ShipmentAddressExistsAsync(request.OriginAddressId, cancellationToken))
            throw new KeyNotFoundException("Origin address not found.");

        if (!await _lookupRepository.ShipmentAddressExistsAsync(request.DestinationAddressId, cancellationToken))
            throw new KeyNotFoundException("Destination address not found.");

        if (request.CarrierId.HasValue && !await _lookupRepository.CarrierExistsAsync(request.CarrierId.Value, cancellationToken))
            throw new KeyNotFoundException("Carrier not found.");

        if (request.CarrierServiceId.HasValue && !await _lookupRepository.CarrierServiceExistsAsync(request.CarrierServiceId.Value, cancellationToken))
            throw new KeyNotFoundException("Carrier service not found.");

        entity.OriginAddressId = request.OriginAddressId;
        entity.DestinationAddressId = request.DestinationAddressId;
        entity.CarrierId = request.CarrierId;
        entity.CarrierServiceId = request.CarrierServiceId;
        entity.TrackingNumber = request.TrackingNumber;
        entity.ReasonCode = request.ReasonCode;
        entity.ReasonDescription = request.ReasonDescription;
        entity.Status = request.Status;
        entity.ReceivedAtUtc = request.ReceivedAtUtc;
        entity.Notes = request.Notes;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = currentUser;

        _returnShipmentRepository.Update(entity);
        await _returnShipmentRepository.SaveChangesAsync(cancellationToken);

        var updated = await _returnShipmentRepository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Updated return shipment could not be reloaded.");

        return Map(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        var entity = await _returnShipmentRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = currentUser;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = currentUser;

        _returnShipmentRepository.Update(entity);
        await _returnShipmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ReturnShipmentItemResponse> AddItemAsync(Guid returnShipmentId, AddReturnShipmentItemRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _addItemValidator.ValidateAndThrowAsync(request, cancellationToken);

        var returnShipment = await _returnShipmentRepository.GetByIdAsync(returnShipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Return shipment not found.");

        var shipmentItem = await _shipmentRepository.GetShipmentItemByIdAsync(request.ShipmentItemId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment item not found.");

        var totalReturned = await _returnShipmentRepository.GetTotalReturnedQuantityForShipmentItemAsync(request.ShipmentItemId, cancellationToken);
        if (totalReturned + request.ReturnedQuantity > shipmentItem.DeliveredQuantity)
            throw new InvalidOperationException("Returned quantity exceeds delivered quantity.");

        var entity = new ReturnShipmentItem
        {
            ReturnShipmentId = returnShipmentId,
            ShipmentItemId = request.ShipmentItemId,
            ReturnedQuantity = request.ReturnedQuantity,
            ReturnCondition = request.ReturnCondition,
            InspectionResult = request.InspectionResult,
            Notes = request.Notes,
            CreatedBy = currentUser
        };

        await _returnShipmentRepository.AddItemAsync(entity, cancellationToken);
        await _returnShipmentRepository.SaveChangesAsync(cancellationToken);

        var created = await _returnShipmentRepository.GetReturnItemByIdAsync(entity.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created return shipment item could not be reloaded.");

        return MapItem(created);
    }

    public async Task<bool> RemoveItemAsync(Guid returnShipmentId, Guid returnShipmentItemId, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        var item = await _returnShipmentRepository.GetReturnItemByIdAsync(returnShipmentItemId, cancellationToken);
        if (item == null || item.ReturnShipmentId != returnShipmentId) return false;

        item.IsDeleted = true;
        item.DeletedAtUtc = DateTime.UtcNow;
        item.DeletedBy = currentUser;
        item.UpdatedAtUtc = DateTime.UtcNow;
        item.UpdatedBy = currentUser;

        _returnShipmentRepository.UpdateItem(item);
        await _returnShipmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ReturnShipmentItemResponse>> GetItemsAsync(Guid returnShipmentId, CancellationToken cancellationToken = default)
    {
        if (!await _returnShipmentRepository.ExistsAsync(x => x.Id == returnShipmentId, cancellationToken))
            throw new KeyNotFoundException("Return shipment not found.");

        var items = await _returnShipmentRepository.GetItemsByReturnShipmentIdAsync(returnShipmentId, cancellationToken);
        return items.Select(MapItem).ToList();
    }

    private static ReturnShipmentResponse Map(ReturnShipment x) => new()
    {
        Id = x.Id,
        ReturnShipmentNumber = x.ReturnShipmentNumber,
        ShipmentId = x.ShipmentId,
        OrderId = x.OrderId,
        OriginAddressId = x.OriginAddressId,
        DestinationAddressId = x.DestinationAddressId,
        CarrierId = x.CarrierId,
        CarrierName = x.Carrier?.Name,
        CarrierServiceId = x.CarrierServiceId,
        CarrierServiceName = x.CarrierService?.Name,
        TrackingNumber = x.TrackingNumber,
        ReasonCode = x.ReasonCode,
        ReasonDescription = x.ReasonDescription,
        Status = x.Status,
        RequestedAtUtc = x.RequestedAtUtc,
        ReceivedAtUtc = x.ReceivedAtUtc,
        Notes = x.Notes,
        Items = x.Items.Select(MapItem).ToList()
    };

    private static ReturnShipmentItemResponse MapItem(ReturnShipmentItem x) => new()
    {
        Id = x.Id,
        ShipmentItemId = x.ShipmentItemId,
        ProductId = x.ShipmentItem?.ProductId ?? Guid.Empty,
        ProductName = x.ShipmentItem?.Product?.Name ?? string.Empty,
        ReturnedQuantity = x.ReturnedQuantity,
        ReturnCondition = x.ReturnCondition,
        InspectionResult = x.InspectionResult,
        Notes = x.Notes
    };
}