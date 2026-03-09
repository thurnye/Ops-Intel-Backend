using FluentValidation;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IShipmentLookupRepository _lookupRepository;
    private readonly IDeliveryRunRepository _deliveryRunRepository;
    private readonly IDockAppointmentRepository _dockAppointmentRepository;
    private readonly IValidator<CreateShipmentRequest> _createShipmentValidator;
    private readonly IValidator<UpdateShipmentRequest> _updateShipmentValidator;
    private readonly IValidator<ShipmentFilterRequest> _shipmentFilterValidator;
    private readonly IValidator<AddShipmentItemRequest> _addShipmentItemValidator;
    private readonly IValidator<UpdateShipmentItemRequest> _updateShipmentItemValidator;
    private readonly IValidator<AddShipmentPackageRequest> _addShipmentPackageValidator;
    private readonly IValidator<UpdateShipmentPackageRequest> _updateShipmentPackageValidator;
    private readonly IValidator<AddShipmentPackageItemRequest> _addShipmentPackageItemValidator;
    private readonly IValidator<AddShipmentTrackingEventRequest> _addShipmentTrackingEventValidator;
    private readonly IValidator<AddShipmentDocumentRequest> _addShipmentDocumentValidator;
    private readonly IValidator<AddShipmentChargeRequest> _addShipmentChargeValidator;
    private readonly IValidator<AddShipmentExceptionRequest> _addShipmentExceptionValidator;
    private readonly IValidator<AddShipmentInsuranceRequest> _addShipmentInsuranceValidator;
    private readonly IValidator<AddCustomsDocumentRequest> _addCustomsDocumentValidator;
    private readonly IValidator<UpdateShipmentStatusRequest> _updateShipmentStatusValidator;
    private readonly IValidator<AssignShipmentToDeliveryRunRequest> _assignShipmentToDeliveryRunValidator;
    private readonly IValidator<AssignShipmentToDockAppointmentRequest> _assignShipmentToDockAppointmentValidator;

    public ShipmentService(
        IShipmentRepository shipmentRepository,
        IShipmentLookupRepository lookupRepository,
        IDeliveryRunRepository deliveryRunRepository,
        IDockAppointmentRepository dockAppointmentRepository,
        IValidator<CreateShipmentRequest> createShipmentValidator,
        IValidator<UpdateShipmentRequest> updateShipmentValidator,
        IValidator<ShipmentFilterRequest> shipmentFilterValidator,
        IValidator<AddShipmentItemRequest> addShipmentItemValidator,
        IValidator<UpdateShipmentItemRequest> updateShipmentItemValidator,
        IValidator<AddShipmentPackageRequest> addShipmentPackageValidator,
        IValidator<UpdateShipmentPackageRequest> updateShipmentPackageValidator,
        IValidator<AddShipmentPackageItemRequest> addShipmentPackageItemValidator,
        IValidator<AddShipmentTrackingEventRequest> addShipmentTrackingEventValidator,
        IValidator<AddShipmentDocumentRequest> addShipmentDocumentValidator,
        IValidator<AddShipmentChargeRequest> addShipmentChargeValidator,
        IValidator<AddShipmentExceptionRequest> addShipmentExceptionValidator,
        IValidator<AddShipmentInsuranceRequest> addShipmentInsuranceValidator,
        IValidator<AddCustomsDocumentRequest> addCustomsDocumentValidator,
        IValidator<UpdateShipmentStatusRequest> updateShipmentStatusValidator,
        IValidator<AssignShipmentToDeliveryRunRequest> assignShipmentToDeliveryRunValidator,
        IValidator<AssignShipmentToDockAppointmentRequest> assignShipmentToDockAppointmentValidator)
    {
        _shipmentRepository = shipmentRepository;
        _lookupRepository = lookupRepository;
        _deliveryRunRepository = deliveryRunRepository;
        _dockAppointmentRepository = dockAppointmentRepository;
        _createShipmentValidator = createShipmentValidator;
        _updateShipmentValidator = updateShipmentValidator;
        _shipmentFilterValidator = shipmentFilterValidator;
        _addShipmentItemValidator = addShipmentItemValidator;
        _updateShipmentItemValidator = updateShipmentItemValidator;
        _addShipmentPackageValidator = addShipmentPackageValidator;
        _updateShipmentPackageValidator = updateShipmentPackageValidator;
        _addShipmentPackageItemValidator = addShipmentPackageItemValidator;
        _addShipmentTrackingEventValidator = addShipmentTrackingEventValidator;
        _addShipmentDocumentValidator = addShipmentDocumentValidator;
        _addShipmentChargeValidator = addShipmentChargeValidator;
        _addShipmentExceptionValidator = addShipmentExceptionValidator;
        _addShipmentInsuranceValidator = addShipmentInsuranceValidator;
        _addCustomsDocumentValidator = addCustomsDocumentValidator;
        _updateShipmentStatusValidator = updateShipmentStatusValidator;
        _assignShipmentToDeliveryRunValidator = assignShipmentToDeliveryRunValidator;
        _assignShipmentToDockAppointmentValidator = assignShipmentToDockAppointmentValidator;
    }

    public async Task<PagedResponse<ShipmentListItemResponse>> GetPagedAsync(
        ShipmentFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        await _shipmentFilterValidator.ValidateAndThrowAsync(request, cancellationToken);

        var items = await _shipmentRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.Status,
            request.Type,
            request.Priority,
            request.OrderId,
            request.WarehouseId,
            request.CarrierId,
            request.PlannedShipDateFromUtc,
            request.PlannedShipDateToUtc,
            request.IsCrossBorder,
            request.IsPartialShipment,
            cancellationToken);

        var total = await _shipmentRepository.CountAsync(
            request.Search,
            request.Status,
            request.Type,
            request.Priority,
            request.OrderId,
            request.WarehouseId,
            request.CarrierId,
            request.PlannedShipDateFromUtc,
            request.PlannedShipDateToUtc,
            request.IsCrossBorder,
            request.IsPartialShipment,
            cancellationToken);

        return new PagedResponse<ShipmentListItemResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = total,
            Items = items.Select(MapShipmentListItem).ToList()
        };
    }

    public async Task<ShipmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return shipment == null ? null : MapShipment(shipment);
    }

    public async Task<ShipmentResponse> CreateAsync(
        CreateShipmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _createShipmentValidator.ValidateAndThrowAsync(request, cancellationToken);

        await EnsureShipmentReferencesExistAsync(
            request.OrderId,
            request.WarehouseId,
            request.OriginAddressId,
            request.DestinationAddressId,
            request.CarrierId,
            request.CarrierServiceId,
            cancellationToken);

        var shipmentNumber = $"SHP-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        var shipment = new Shipment
        {
            ShipmentNumber = shipmentNumber,
            OrderId = request.OrderId,
            WarehouseId = request.WarehouseId,
            OriginAddressId = request.OriginAddressId,
            DestinationAddressId = request.DestinationAddressId,
            CarrierId = request.CarrierId,
            CarrierServiceId = request.CarrierServiceId,
            Type = request.Type,
            Priority = request.Priority,
            CustomerReference = request.CustomerReference,
            ExternalReference = request.ExternalReference,
            TrackingNumber = request.TrackingNumber,
            MasterTrackingNumber = request.MasterTrackingNumber,
            PlannedShipDateUtc = request.PlannedShipDateUtc,
            PlannedDeliveryDateUtc = request.PlannedDeliveryDateUtc,
            ScheduledPickupStartUtc = request.ScheduledPickupStartUtc,
            ScheduledPickupEndUtc = request.ScheduledPickupEndUtc,
            IsPartialShipment = request.IsPartialShipment,
            RequiresSignature = request.RequiresSignature,
            IsFragile = request.IsFragile,
            IsHazardous = request.IsHazardous,
            IsTemperatureControlled = request.IsTemperatureControlled,
            IsInsured = request.IsInsured,
            IsCrossBorder = request.IsCrossBorder,
            CurrencyCode = request.CurrencyCode,
            ShippingTerms = request.ShippingTerms,
            Incoterm = request.Incoterm,
            Notes = request.Notes,
            InternalNotes = request.InternalNotes,
            Status = ShipmentStatus.Draft,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddAsync(shipment, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        var created = await _shipmentRepository.GetByIdWithDetailsAsync(shipment.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created shipment could not be reloaded.");

        return MapShipment(created);
    }

    public async Task<ShipmentResponse> UpdateAsync(
        Guid id,
        UpdateShipmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _updateShipmentValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureShipmentEditable(shipment);

        await EnsureShipmentReferencesExistAsync(
            shipment.OrderId,
            shipment.WarehouseId,
            request.OriginAddressId,
            request.DestinationAddressId,
            request.CarrierId,
            request.CarrierServiceId,
            cancellationToken);

        shipment.CarrierId = request.CarrierId;
        shipment.CarrierServiceId = request.CarrierServiceId;
        shipment.OriginAddressId = request.OriginAddressId;
        shipment.DestinationAddressId = request.DestinationAddressId;
        shipment.Priority = request.Priority;
        shipment.CustomerReference = request.CustomerReference;
        shipment.ExternalReference = request.ExternalReference;
        shipment.TrackingNumber = request.TrackingNumber;
        shipment.MasterTrackingNumber = request.MasterTrackingNumber;
        shipment.PlannedShipDateUtc = request.PlannedShipDateUtc;
        shipment.PlannedDeliveryDateUtc = request.PlannedDeliveryDateUtc;
        shipment.ScheduledPickupStartUtc = request.ScheduledPickupStartUtc;
        shipment.ScheduledPickupEndUtc = request.ScheduledPickupEndUtc;
        shipment.RequiresSignature = request.RequiresSignature;
        shipment.IsFragile = request.IsFragile;
        shipment.IsHazardous = request.IsHazardous;
        shipment.IsTemperatureControlled = request.IsTemperatureControlled;
        shipment.IsInsured = request.IsInsured;
        shipment.IsCrossBorder = request.IsCrossBorder;
        shipment.CurrencyCode = request.CurrencyCode;
        shipment.ShippingTerms = request.ShippingTerms;
        shipment.Incoterm = request.Incoterm;
        shipment.Notes = request.Notes;
        shipment.InternalNotes = request.InternalNotes;
        shipment.UpdatedAtUtc = DateTime.UtcNow;
        shipment.UpdatedBy = currentUser;

        _shipmentRepository.Update(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        var updated = await _shipmentRepository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Updated shipment could not be reloaded.");

        return MapShipment(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(id, cancellationToken);
        if (shipment == null) return false;

        EnsureShipmentEditable(shipment);

        shipment.IsDeleted = true;
        shipment.DeletedAtUtc = DateTime.UtcNow;
        shipment.DeletedBy = currentUser;
        shipment.UpdatedAtUtc = DateTime.UtcNow;
        shipment.UpdatedBy = currentUser;

        _shipmentRepository.Update(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ShipmentItemResponse> AddItemAsync(
        Guid shipmentId,
        AddShipmentItemRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _addShipmentItemValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureShipmentEditable(shipment);

        await EnsureShipmentItemReferencesExistAsync(request, cancellationToken);

        if (request.OrderItemId.HasValue)
        {
            var totalShipped = await _shipmentRepository.GetTotalShippedQuantityForOrderItemAsync(request.OrderItemId.Value, cancellationToken);
            if (totalShipped + request.ShippedQuantity > request.OrderedQuantity)
            {
                throw new InvalidOperationException("Shipped quantity exceeds the allowed quantity for the order item.");
            }
        }

        var item = new ShipmentItem
        {
            ShipmentId = shipmentId,
            OrderItemId = request.OrderItemId,
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            UnitOfMeasureId = request.UnitOfMeasureId,
            InventoryStockId = request.InventoryStockId,
            ProductionOrderId = request.ProductionOrderId,
            LineNumber = request.LineNumber,
            OrderedQuantity = request.OrderedQuantity,
            AllocatedQuantity = request.AllocatedQuantity,
            PickedQuantity = request.PickedQuantity,
            PackedQuantity = request.PackedQuantity,
            ShippedQuantity = request.ShippedQuantity,
            UnitWeight = request.UnitWeight,
            UnitVolume = request.UnitVolume,
            LotNumber = request.LotNumber,
            SerialNumber = request.SerialNumber,
            ExpiryDateUtc = request.ExpiryDateUtc,
            Status = request.ShippedQuantity > 0 ? ShipmentItemStatus.Shipped : ShipmentItemStatus.Pending,
            Notes = request.Notes,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddItemAsync(item, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
        await RecalculateShipmentTotalsAsync(shipmentId, currentUser, cancellationToken);

        var created = await _shipmentRepository.GetShipmentItemByIdAsync(item.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created shipment item could not be reloaded.");

        return MapShipmentItem(created);
    }

    public async Task<ShipmentItemResponse> UpdateItemAsync(
        Guid shipmentId,
        Guid shipmentItemId,
        UpdateShipmentItemRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _updateShipmentItemValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureShipmentEditable(shipment);

        var item = await _shipmentRepository.GetShipmentItemByIdAsync(shipmentItemId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment item not found.");

        if (item.ShipmentId != shipmentId)
            throw new InvalidOperationException("Shipment item does not belong to shipment.");

        if (!await _lookupRepository.WarehouseExistsAsync(request.WarehouseId, cancellationToken))
            throw new KeyNotFoundException("Warehouse not found.");

        if (!await _lookupRepository.UnitOfMeasureExistsAsync(request.UnitOfMeasureId, cancellationToken))
            throw new KeyNotFoundException("Unit of measure not found.");

        if (request.InventoryStockId.HasValue &&
            !await _lookupRepository.InventoryStockExistsAsync(request.InventoryStockId.Value, cancellationToken))
            throw new KeyNotFoundException("Inventory stock not found.");

        if (request.ProductionOrderId.HasValue &&
            !await _lookupRepository.ProductionOrderExistsAsync(request.ProductionOrderId.Value, cancellationToken))
            throw new KeyNotFoundException("Production order not found.");

        if (item.OrderItemId.HasValue)
        {
            var totalShipped = await _shipmentRepository.GetTotalShippedQuantityForOrderItemAsync(item.OrderItemId.Value, cancellationToken);
            var adjustedTotal = totalShipped - item.ShippedQuantity + request.ShippedQuantity;
            if (adjustedTotal > request.OrderedQuantity)
            {
                throw new InvalidOperationException("Shipped quantity exceeds the allowed quantity for the order item.");
            }
        }

        item.WarehouseId = request.WarehouseId;
        item.UnitOfMeasureId = request.UnitOfMeasureId;
        item.InventoryStockId = request.InventoryStockId;
        item.ProductionOrderId = request.ProductionOrderId;
        item.OrderedQuantity = request.OrderedQuantity;
        item.AllocatedQuantity = request.AllocatedQuantity;
        item.PickedQuantity = request.PickedQuantity;
        item.PackedQuantity = request.PackedQuantity;
        item.ShippedQuantity = request.ShippedQuantity;
        item.DeliveredQuantity = request.DeliveredQuantity;
        item.ReturnedQuantity = request.ReturnedQuantity;
        item.UnitWeight = request.UnitWeight;
        item.UnitVolume = request.UnitVolume;
        item.LotNumber = request.LotNumber;
        item.SerialNumber = request.SerialNumber;
        item.ExpiryDateUtc = request.ExpiryDateUtc;
        item.Status = request.Status;
        item.Notes = request.Notes;
        item.UpdatedAtUtc = DateTime.UtcNow;
        item.UpdatedBy = currentUser;

        _shipmentRepository.UpdateItem(item);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
        await RecalculateShipmentTotalsAsync(shipmentId, currentUser, cancellationToken);

        var updated = await _shipmentRepository.GetShipmentItemByIdAsync(item.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Updated shipment item could not be reloaded.");

        return MapShipmentItem(updated);
    }

    public async Task<bool> RemoveItemAsync(
        Guid shipmentId,
        Guid shipmentItemId,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken);
        if (shipment == null) return false;

        EnsureShipmentEditable(shipment);

        var item = await _shipmentRepository.GetShipmentItemByIdAsync(shipmentItemId, cancellationToken);
        if (item == null || item.ShipmentId != shipmentId) return false;

        item.IsDeleted = true;
        item.DeletedAtUtc = DateTime.UtcNow;
        item.DeletedBy = currentUser;
        item.UpdatedAtUtc = DateTime.UtcNow;
        item.UpdatedBy = currentUser;

        _shipmentRepository.UpdateItem(item);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
        await RecalculateShipmentTotalsAsync(shipmentId, currentUser, cancellationToken);
        return true;
    }

    public async Task<ShipmentPackageResponse> AddPackageAsync(
        Guid shipmentId,
        AddShipmentPackageRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _addShipmentPackageValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureShipmentEditable(shipment);

        var package = new ShipmentPackage
        {
            ShipmentId = shipmentId,
            PackageNumber = request.PackageNumber,
            PackageType = request.PackageType,
            Length = request.Length,
            Width = request.Width,
            Height = request.Height,
            Weight = request.Weight,
            DeclaredValue = request.DeclaredValue,
            RequiresSpecialHandling = request.RequiresSpecialHandling,
            IsFragile = request.IsFragile,
            TrackingNumber = request.TrackingNumber,
            Barcode = request.Barcode,
            LabelUrl = request.LabelUrl,
            Status = PackageStatus.Draft,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddPackageAsync(package, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
        await RecalculateShipmentTotalsAsync(shipmentId, currentUser, cancellationToken);

        var created = await _shipmentRepository.GetShipmentPackageByIdAsync(package.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created shipment package could not be reloaded.");

        return MapShipmentPackage(created);
    }

    public async Task<ShipmentPackageResponse> UpdatePackageAsync(
        Guid shipmentId,
        Guid shipmentPackageId,
        UpdateShipmentPackageRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _updateShipmentPackageValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureShipmentEditable(shipment);

        var package = await _shipmentRepository.GetShipmentPackageByIdAsync(shipmentPackageId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment package not found.");

        if (package.ShipmentId != shipmentId)
            throw new InvalidOperationException("Shipment package does not belong to shipment.");

        package.PackageType = request.PackageType;
        package.TrackingNumber = request.TrackingNumber;
        package.Length = request.Length;
        package.Width = request.Width;
        package.Height = request.Height;
        package.Weight = request.Weight;
        package.DeclaredValue = request.DeclaredValue;
        package.RequiresSpecialHandling = request.RequiresSpecialHandling;
        package.IsFragile = request.IsFragile;
        package.Barcode = request.Barcode;
        package.LabelUrl = request.LabelUrl;
        package.Status = request.Status;
        package.UpdatedAtUtc = DateTime.UtcNow;
        package.UpdatedBy = currentUser;

        _shipmentRepository.UpdatePackage(package);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
        await RecalculateShipmentTotalsAsync(shipmentId, currentUser, cancellationToken);

        var updated = await _shipmentRepository.GetShipmentPackageByIdAsync(package.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Updated shipment package could not be reloaded.");

        return MapShipmentPackage(updated);
    }

    public async Task<bool> RemovePackageAsync(
        Guid shipmentId,
        Guid shipmentPackageId,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken);
        if (shipment == null) return false;

        EnsureShipmentEditable(shipment);

        var package = await _shipmentRepository.GetShipmentPackageByIdAsync(shipmentPackageId, cancellationToken);
        if (package == null || package.ShipmentId != shipmentId) return false;

        package.IsDeleted = true;
        package.DeletedAtUtc = DateTime.UtcNow;
        package.DeletedBy = currentUser;
        package.UpdatedAtUtc = DateTime.UtcNow;
        package.UpdatedBy = currentUser;

        _shipmentRepository.UpdatePackage(package);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
        await RecalculateShipmentTotalsAsync(shipmentId, currentUser, cancellationToken);
        return true;
    }

    public async Task<ShipmentPackageItemResponse> AddPackageItemAsync(
        Guid shipmentId,
        Guid shipmentPackageId,
        AddShipmentPackageItemRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _addShipmentPackageItemValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");
        EnsureShipmentEditable(shipment);

        var package = await _shipmentRepository.GetShipmentPackageByIdAsync(shipmentPackageId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment package not found.");
        if (package.ShipmentId != shipmentId)
            throw new InvalidOperationException("Shipment package does not belong to shipment.");

        var item = await _shipmentRepository.GetShipmentItemByIdAsync(request.ShipmentItemId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment item not found.");
        if (item.ShipmentId != shipmentId)
            throw new InvalidOperationException("Shipment item does not belong to shipment.");

        var packageItems = await _shipmentRepository.GetPackagesByShipmentIdAsync(shipmentId, cancellationToken);
        var packagedQty = packageItems
            .SelectMany(x => x.PackageItems)
            .Where(x => x.ShipmentItemId == request.ShipmentItemId)
            .Sum(x => x.Quantity);

        if (packagedQty + request.Quantity > item.PackedQuantity)
            throw new InvalidOperationException("Package quantity exceeds available packed quantity.");

        var packageItem = new ShipmentPackageItem
        {
            ShipmentPackageId = shipmentPackageId,
            ShipmentItemId = request.ShipmentItemId,
            Quantity = request.Quantity,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddPackageItemAsync(packageItem, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        var created = await _shipmentRepository.GetShipmentPackageItemByIdAsync(packageItem.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created package item could not be reloaded.");

        return MapShipmentPackageItem(created);
    }

    public async Task<bool> RemovePackageItemAsync(
        Guid shipmentId,
        Guid shipmentPackageId,
        Guid shipmentPackageItemId,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken);
        if (shipment == null) return false;
        EnsureShipmentEditable(shipment);

        var package = await _shipmentRepository.GetShipmentPackageByIdAsync(shipmentPackageId, cancellationToken);
        if (package == null || package.ShipmentId != shipmentId) return false;

        var packageItem = await _shipmentRepository.GetShipmentPackageItemByIdAsync(shipmentPackageItemId, cancellationToken);
        if (packageItem == null || packageItem.ShipmentPackageId != shipmentPackageId) return false;

        packageItem.IsDeleted = true;
        packageItem.DeletedAtUtc = DateTime.UtcNow;
        packageItem.DeletedBy = currentUser;
        packageItem.UpdatedAtUtc = DateTime.UtcNow;
        packageItem.UpdatedBy = currentUser;

        _shipmentRepository.UpdatePackageItem(packageItem);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ShipmentTrackingEventResponse> AddTrackingEventAsync(
        Guid shipmentId,
        AddShipmentTrackingEventRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _addShipmentTrackingEventValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var trackingEvent = new ShipmentTrackingEvent
        {
            ShipmentId = shipmentId,
            EventCode = request.EventCode,
            EventName = request.EventName,
            Description = request.Description,
            EventTimeUtc = request.EventTimeUtc,
            LocationName = request.LocationName,
            City = request.City,
            StateOrProvince = request.StateOrProvince,
            Country = request.Country,
            CarrierStatusCode = request.CarrierStatusCode,
            Source = request.Source,
            IsCustomerVisible = request.IsCustomerVisible,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddTrackingEventAsync(trackingEvent, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        return MapTrackingEvent(trackingEvent);
    }

    public async Task<ShipmentDocumentResponse> AddDocumentAsync(
        Guid shipmentId,
        AddShipmentDocumentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _addShipmentDocumentValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var document = new ShipmentDocument
        {
            ShipmentId = shipmentId,
            DocumentType = request.DocumentType,
            FileName = request.FileName,
            FileUrl = request.FileUrl,
            ContentType = request.ContentType,
            FileSizeBytes = request.FileSizeBytes,
            IsCustomerVisible = request.IsCustomerVisible,
            Notes = request.Notes,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddDocumentAsync(document, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        return MapDocument(document);
    }

    public async Task<ShipmentChargeResponse> AddChargeAsync(
        Guid shipmentId,
        AddShipmentChargeRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _addShipmentChargeValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var charge = new ShipmentCharge
        {
            ShipmentId = shipmentId,
            ChargeType = request.ChargeType,
            Description = request.Description,
            Amount = request.Amount,
            CurrencyCode = request.CurrencyCode,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddChargeAsync(charge, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
        await RecalculateShipmentTotalsAsync(shipmentId, currentUser, cancellationToken);

        return MapCharge(charge);
    }

    public async Task<ShipmentExceptionResponse> AddExceptionAsync(
        Guid shipmentId,
        AddShipmentExceptionRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _addShipmentExceptionValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var shipmentException = new ShipmentException
        {
            ShipmentId = shipmentId,
            ExceptionType = request.ExceptionType,
            Title = request.Title,
            Description = request.Description,
            ReportedAtUtc = request.ReportedAtUtc,
            ReportedBy = request.ReportedBy,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddExceptionAsync(shipmentException, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        return MapException(shipmentException);
    }

    public async Task<ShipmentInsuranceResponse> AddInsuranceAsync(
        Guid shipmentId,
        AddShipmentInsuranceRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _addShipmentInsuranceValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        if (!shipment.IsInsured)
            throw new InvalidOperationException("Shipment is not marked as insured.");

        var insurance = new ShipmentInsurance
        {
            ShipmentId = shipmentId,
            ProviderName = request.ProviderName,
            PolicyNumber = request.PolicyNumber,
            InsuredAmount = request.InsuredAmount,
            PremiumAmount = request.PremiumAmount,
            CurrencyCode = request.CurrencyCode,
            EffectiveDateUtc = request.EffectiveDateUtc,
            ExpiryDateUtc = request.ExpiryDateUtc,
            Status = InsuranceStatus.Active,
            Notes = request.Notes,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddInsuranceAsync(insurance, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        return MapInsurance(insurance);
    }

    public async Task<CustomsDocumentResponse> AddCustomsDocumentAsync(
        Guid shipmentId,
        AddCustomsDocumentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _addCustomsDocumentValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        if (!shipment.IsCrossBorder)
            throw new InvalidOperationException("Customs documents can only be added to cross-border shipments.");

        var customsDocument = new CustomsDocument
        {
            ShipmentId = shipmentId,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            FileName = request.FileName,
            FileUrl = request.FileUrl,
            CountryOfOrigin = request.CountryOfOrigin,
            DestinationCountry = request.DestinationCountry,
            HarmonizedCode = request.HarmonizedCode,
            DeclaredCustomsValue = request.DeclaredCustomsValue,
            CurrencyCode = request.CurrencyCode,
            IssuedAtUtc = request.IssuedAtUtc,
            Notes = request.Notes,
            CreatedBy = currentUser
        };

        await _shipmentRepository.AddCustomsDocumentAsync(customsDocument, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        return MapCustomsDocument(customsDocument);
    }

    public async Task<ShipmentResponse> UpdateStatusAsync(
        Guid shipmentId,
        UpdateShipmentStatusRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _updateShipmentStatusValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureValidStatusTransition(shipment.Status, request.Status);

        var oldStatus = shipment.Status;
        shipment.Status = request.Status;

        if (request.Status == ShipmentStatus.Dispatched && shipment.ActualShipDateUtc == null)
            shipment.ActualShipDateUtc = DateTime.UtcNow;

        if (request.Status == ShipmentStatus.Delivered && shipment.ActualDeliveryDateUtc == null)
            shipment.ActualDeliveryDateUtc = DateTime.UtcNow;

        shipment.UpdatedAtUtc = DateTime.UtcNow;
        shipment.UpdatedBy = currentUser;

        _shipmentRepository.Update(shipment);

        await _shipmentRepository.AddStatusHistoryAsync(new ShipmentStatusHistory
        {
            ShipmentId = shipmentId,
            FromStatus = oldStatus,
            ToStatus = request.Status,
            ChangedAtUtc = DateTime.UtcNow,
            ChangedBy = currentUser ?? "system",
            Reason = request.Reason,
            CreatedBy = currentUser
        }, cancellationToken);

        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        var updated = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment could not be reloaded after status update.");

        return MapShipment(updated);
    }

    public async Task<ShipmentResponse> AssignToDeliveryRunAsync(
        Guid shipmentId,
        AssignShipmentToDeliveryRunRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _assignShipmentToDeliveryRunValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureShipmentEditable(shipment);

        var run = await _deliveryRunRepository.GetByIdAsync(request.DeliveryRunId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery run not found.");

        if (shipment.WarehouseId != run.WarehouseId)
            throw new InvalidOperationException("Shipment warehouse must match delivery run warehouse.");

        shipment.DeliveryRunId = request.DeliveryRunId;
        shipment.UpdatedAtUtc = DateTime.UtcNow;
        shipment.UpdatedBy = currentUser;

        _shipmentRepository.Update(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        var updated = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment could not be reloaded after assigning delivery run.");

        return MapShipment(updated);
    }

    public async Task<ShipmentResponse> UnassignFromDeliveryRunAsync(
        Guid shipmentId,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureShipmentEditable(shipment);

        shipment.DeliveryRunId = null;
        shipment.UpdatedAtUtc = DateTime.UtcNow;
        shipment.UpdatedBy = currentUser;

        _shipmentRepository.Update(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        var updated = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment could not be reloaded after unassigning delivery run.");

        return MapShipment(updated);
    }

    public async Task<ShipmentResponse> AssignToDockAppointmentAsync(
        Guid shipmentId,
        AssignShipmentToDockAppointmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        await _assignShipmentToDockAppointmentValidator.ValidateAndThrowAsync(request, cancellationToken);

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureShipmentEditable(shipment);

        var appointment = await _dockAppointmentRepository.GetByIdAsync(request.DockAppointmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Dock appointment not found.");

        if (shipment.WarehouseId != appointment.WarehouseId)
            throw new InvalidOperationException("Shipment warehouse must match dock appointment warehouse.");

        if (appointment.CarrierId.HasValue && shipment.CarrierId.HasValue &&
            appointment.CarrierId.Value != shipment.CarrierId.Value)
            throw new InvalidOperationException("Shipment carrier must match dock appointment carrier when both are set.");

        shipment.DockAppointmentId = request.DockAppointmentId;
        shipment.UpdatedAtUtc = DateTime.UtcNow;
        shipment.UpdatedBy = currentUser;

        _shipmentRepository.Update(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        var updated = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment could not be reloaded after assigning dock appointment.");

        return MapShipment(updated);
    }

    public async Task<ShipmentResponse> UnassignFromDockAppointmentAsync(
        Guid shipmentId,
        string? currentUser = null,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        EnsureShipmentEditable(shipment);

        shipment.DockAppointmentId = null;
        shipment.UpdatedAtUtc = DateTime.UtcNow;
        shipment.UpdatedBy = currentUser;

        _shipmentRepository.Update(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        var updated = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment could not be reloaded after unassigning dock appointment.");

        return MapShipment(updated);
    }

    public async Task<IReadOnlyList<ShipmentItemResponse>> GetItemsAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var items = await _shipmentRepository.GetItemsByShipmentIdAsync(shipment.Id, cancellationToken);
        return items.Select(MapShipmentItem).ToList();
    }

    public async Task<IReadOnlyList<ShipmentPackageResponse>> GetPackagesAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var packages = await _shipmentRepository.GetPackagesByShipmentIdAsync(shipment.Id, cancellationToken);
        return packages.Select(MapShipmentPackage).ToList();
    }

    public async Task<IReadOnlyList<ShipmentTrackingEventResponse>> GetTrackingEventsAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var events = await _shipmentRepository.GetTrackingEventsByShipmentIdAsync(shipment.Id, cancellationToken);
        return events.Select(MapTrackingEvent).ToList();
    }

    public async Task<IReadOnlyList<ShipmentDocumentResponse>> GetDocumentsAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var documents = await _shipmentRepository.GetDocumentsByShipmentIdAsync(shipment.Id, cancellationToken);
        return documents.Select(MapDocument).ToList();
    }

    public async Task<IReadOnlyList<ShipmentChargeResponse>> GetChargesAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var charges = await _shipmentRepository.GetChargesByShipmentIdAsync(shipment.Id, cancellationToken);
        return charges.Select(MapCharge).ToList();
    }

    public async Task<IReadOnlyList<ShipmentExceptionResponse>> GetExceptionsAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var exceptions = await _shipmentRepository.GetExceptionsByShipmentIdAsync(shipment.Id, cancellationToken);
        return exceptions.Select(MapException).ToList();
    }

    public async Task<IReadOnlyList<ShipmentInsuranceResponse>> GetInsurancesAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var insurances = await _shipmentRepository.GetInsurancesByShipmentIdAsync(shipment.Id, cancellationToken);
        return insurances.Select(MapInsurance).ToList();
    }

    public async Task<IReadOnlyList<ShipmentStatusHistoryResponse>> GetStatusHistoryAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var history = await _shipmentRepository.GetStatusHistoryByShipmentIdAsync(shipment.Id, cancellationToken);
        return history.Select(MapStatusHistory).ToList();
    }

    public async Task<IReadOnlyList<CustomsDocumentResponse>> GetCustomsDocumentsAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found.");

        var documents = await _shipmentRepository.GetCustomsDocumentsByShipmentIdAsync(shipment.Id, cancellationToken);
        return documents.Select(MapCustomsDocument).ToList();
    }

    public async Task<ShipmentSummaryResponse> GetSummaryAsync(ShipmentFilterRequest request, CancellationToken cancellationToken = default)
    {
        await _shipmentFilterValidator.ValidateAndThrowAsync(request, cancellationToken);

        var all = await _shipmentRepository.GetPagedAsync(
            1,
            100000,
            request.Search,
            request.Status,
            request.Type,
            request.Priority,
            request.OrderId,
            request.WarehouseId,
            request.CarrierId,
            request.PlannedShipDateFromUtc,
            request.PlannedShipDateToUtc,
            request.IsCrossBorder,
            request.IsPartialShipment,
            cancellationToken);

        return new ShipmentSummaryResponse
        {
            TotalShipments = all.Count,
            DraftShipments = all.Count(x => x.Status == ShipmentStatus.Draft),
            ReadyToDispatchShipments = all.Count(x => x.Status == ShipmentStatus.ReadyToDispatch),
            InTransitShipments = all.Count(x => x.Status == ShipmentStatus.InTransit),
            DeliveredShipments = all.Count(x => x.Status == ShipmentStatus.Delivered),
            FailedShipments = all.Count(x => x.Status == ShipmentStatus.DeliveryFailed),
            ReturnedShipments = all.Count(x => x.Status == ShipmentStatus.Returned),
            TotalFreightCost = all.Sum(x => x.FreightCost),
            TotalShippingCost = all.Sum(x => x.TotalShippingCost),
            TotalPackages = all.Sum(x => x.TotalPackages),
            TotalWeight = all.Sum(x => x.TotalWeight)
        };
    }

    private async Task EnsureShipmentReferencesExistAsync(
        Guid? orderId,
        Guid warehouseId,
        Guid originAddressId,
        Guid destinationAddressId,
        Guid? carrierId,
        Guid? carrierServiceId,
        CancellationToken cancellationToken)
    {
        if (orderId.HasValue && !await _lookupRepository.OrderExistsAsync(orderId.Value, cancellationToken))
            throw new KeyNotFoundException("Order not found.");

        if (!await _lookupRepository.WarehouseExistsAsync(warehouseId, cancellationToken))
            throw new KeyNotFoundException("Warehouse not found.");

        if (!await _lookupRepository.ShipmentAddressExistsAsync(originAddressId, cancellationToken))
            throw new KeyNotFoundException("Origin address not found.");

        if (!await _lookupRepository.ShipmentAddressExistsAsync(destinationAddressId, cancellationToken))
            throw new KeyNotFoundException("Destination address not found.");

        if (carrierId.HasValue && !await _lookupRepository.CarrierExistsAsync(carrierId.Value, cancellationToken))
            throw new KeyNotFoundException("Carrier not found.");

        if (carrierServiceId.HasValue && !await _lookupRepository.CarrierServiceExistsAsync(carrierServiceId.Value, cancellationToken))
            throw new KeyNotFoundException("Carrier service not found.");
    }

    private async Task EnsureShipmentItemReferencesExistAsync(AddShipmentItemRequest request, CancellationToken cancellationToken)
    {
        if (request.OrderItemId.HasValue && !await _lookupRepository.OrderItemExistsAsync(request.OrderItemId.Value, cancellationToken))
            throw new KeyNotFoundException("Order item not found.");

        if (!await _lookupRepository.ProductExistsAsync(request.ProductId, cancellationToken))
            throw new KeyNotFoundException("Product not found.");

        if (!await _lookupRepository.WarehouseExistsAsync(request.WarehouseId, cancellationToken))
            throw new KeyNotFoundException("Warehouse not found.");

        if (!await _lookupRepository.UnitOfMeasureExistsAsync(request.UnitOfMeasureId, cancellationToken))
            throw new KeyNotFoundException("Unit of measure not found.");

        if (request.InventoryStockId.HasValue && !await _lookupRepository.InventoryStockExistsAsync(request.InventoryStockId.Value, cancellationToken))
            throw new KeyNotFoundException("Inventory stock not found.");

        if (request.ProductionOrderId.HasValue && !await _lookupRepository.ProductionOrderExistsAsync(request.ProductionOrderId.Value, cancellationToken))
            throw new KeyNotFoundException("Production order not found.");
    }

    private static void EnsureShipmentEditable(Shipment shipment)
    {
        if (shipment.Status is ShipmentStatus.Delivered or ShipmentStatus.Cancelled or ShipmentStatus.Returned)
            throw new InvalidOperationException("Shipment can no longer be modified in its current status.");
    }

    private static void EnsureValidStatusTransition(ShipmentStatus currentStatus, ShipmentStatus newStatus)
    {
        if (currentStatus == newStatus) return;

        var allowed = currentStatus switch
        {
            ShipmentStatus.Draft => new[] { ShipmentStatus.AwaitingAllocation, ShipmentStatus.Cancelled },
            ShipmentStatus.AwaitingAllocation => new[] { ShipmentStatus.Allocated, ShipmentStatus.Cancelled },
            ShipmentStatus.Allocated => new[] { ShipmentStatus.Picking, ShipmentStatus.Cancelled },
            ShipmentStatus.Picking => new[] { ShipmentStatus.Picked, ShipmentStatus.Cancelled },
            ShipmentStatus.Picked => new[] { ShipmentStatus.Packing, ShipmentStatus.Cancelled },
            ShipmentStatus.Packing => new[] { ShipmentStatus.Packed, ShipmentStatus.Cancelled },
            ShipmentStatus.Packed => new[] { ShipmentStatus.ReadyToDispatch, ShipmentStatus.Cancelled },
            ShipmentStatus.ReadyToDispatch => new[] { ShipmentStatus.Dispatched, ShipmentStatus.Cancelled },
            ShipmentStatus.Dispatched => new[] { ShipmentStatus.InTransit, ShipmentStatus.DeliveryFailed },
            ShipmentStatus.InTransit => new[] { ShipmentStatus.OutForDelivery, ShipmentStatus.DeliveryFailed, ShipmentStatus.Returned },
            ShipmentStatus.OutForDelivery => new[] { ShipmentStatus.Delivered, ShipmentStatus.DeliveryFailed, ShipmentStatus.Returned },
            ShipmentStatus.DeliveryFailed => new[] { ShipmentStatus.OutForDelivery, ShipmentStatus.Returned, ShipmentStatus.Cancelled },
            ShipmentStatus.Delivered => Array.Empty<ShipmentStatus>(),
            ShipmentStatus.Returned => Array.Empty<ShipmentStatus>(),
            ShipmentStatus.Cancelled => Array.Empty<ShipmentStatus>(),
            _ => Array.Empty<ShipmentStatus>()
        };

        if (!allowed.Contains(newStatus))
            throw new InvalidOperationException($"Invalid shipment status transition from {currentStatus} to {newStatus}.");
    }

    private async Task RecalculateShipmentTotalsAsync(Guid shipmentId, string? currentUser, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment not found for recalculation.");

        shipment.TotalPackages = shipment.Packages.Count;
        shipment.TotalWeight = shipment.Packages.Sum(x => x.Weight);
        shipment.TotalVolume = shipment.Packages.Sum(x => x.Length * x.Width * x.Height);
        shipment.FreightCost = shipment.Charges.Where(x => x.ChargeType == ShipmentChargeType.Freight).Sum(x => x.Amount);
        shipment.InsuranceCost = shipment.Charges.Where(x => x.ChargeType == ShipmentChargeType.Insurance).Sum(x => x.Amount);
        shipment.OtherCharges = shipment.Charges.Where(x => x.ChargeType != ShipmentChargeType.Freight && x.ChargeType != ShipmentChargeType.Insurance).Sum(x => x.Amount);
        shipment.TotalShippingCost = shipment.Charges.Sum(x => x.Amount);
        shipment.UpdatedAtUtc = DateTime.UtcNow;
        shipment.UpdatedBy = currentUser;

        _shipmentRepository.Update(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
    }

    private static ShipmentListItemResponse MapShipmentListItem(Shipment x) => new()
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
    };

    private static ShipmentResponse MapShipment(Shipment x) => new()
    {
        Id = x.Id,
        ShipmentNumber = x.ShipmentNumber,
        OrderId = x.OrderId,
        OrderNumber = x.Order?.OrderNumber,
        WarehouseId = x.WarehouseId,
        WarehouseName = x.Warehouse?.Name ?? string.Empty,
        CarrierId = x.CarrierId,
        CarrierName = x.Carrier?.Name,
        CarrierServiceId = x.CarrierServiceId,
        CarrierServiceName = x.CarrierService?.Name,
        OriginAddress = x.OriginAddress == null ? new ShipmentAddressResponse() : MapAddress(x.OriginAddress),
        DestinationAddress = x.DestinationAddress == null ? new ShipmentAddressResponse() : MapAddress(x.DestinationAddress),
        Type = x.Type,
        Status = x.Status,
        Priority = x.Priority,
        CustomerReference = x.CustomerReference,
        ExternalReference = x.ExternalReference,
        TrackingNumber = x.TrackingNumber,
        MasterTrackingNumber = x.MasterTrackingNumber,
        PlannedShipDateUtc = x.PlannedShipDateUtc,
        PlannedDeliveryDateUtc = x.PlannedDeliveryDateUtc,
        ActualShipDateUtc = x.ActualShipDateUtc,
        ActualDeliveryDateUtc = x.ActualDeliveryDateUtc,
        ScheduledPickupStartUtc = x.ScheduledPickupStartUtc,
        ScheduledPickupEndUtc = x.ScheduledPickupEndUtc,
        TotalWeight = x.TotalWeight,
        TotalVolume = x.TotalVolume,
        TotalPackages = x.TotalPackages,
        FreightCost = x.FreightCost,
        InsuranceCost = x.InsuranceCost,
        OtherCharges = x.OtherCharges,
        TotalShippingCost = x.TotalShippingCost,
        CurrencyCode = x.CurrencyCode,
        ShippingTerms = x.ShippingTerms,
        Incoterm = x.Incoterm,
        IsPartialShipment = x.IsPartialShipment,
        RequiresSignature = x.RequiresSignature,
        IsFragile = x.IsFragile,
        IsHazardous = x.IsHazardous,
        IsTemperatureControlled = x.IsTemperatureControlled,
        IsInsured = x.IsInsured,
        IsCrossBorder = x.IsCrossBorder,
        Notes = x.Notes,
        InternalNotes = x.InternalNotes,
        DeliveryRun = x.DeliveryRun == null ? null : MapDeliveryRun(x.DeliveryRun),
        DockAppointment = x.DockAppointment == null ? null : MapDockAppointment(x.DockAppointment),
        Items = x.Items.Select(MapShipmentItem).ToList(),
        Packages = x.Packages.Select(MapShipmentPackage).ToList(),
        TrackingEvents = x.TrackingEvents.Select(MapTrackingEvent).ToList(),
        Documents = x.Documents.Select(MapDocument).ToList(),
        Charges = x.Charges.Select(MapCharge).ToList(),
        Exceptions = x.Exceptions.Select(MapException).ToList(),
        Insurances = x.Insurances.Select(MapInsurance).ToList(),
        StatusHistories = x.StatusHistories.Select(MapStatusHistory).ToList(),
        CustomsDocuments = x.CustomsDocuments.Select(MapCustomsDocument).ToList(),
        ReturnShipments = x.ReturnShipments.Select(MapReturnShipment).ToList(),
        CreatedAtUtc = x.CreatedAtUtc,
        CreatedBy = x.CreatedBy,
        UpdatedAtUtc = x.UpdatedAtUtc,
        UpdatedBy = x.UpdatedBy
    };

    private static ShipmentItemResponse MapShipmentItem(ShipmentItem x) => new()
    {
        Id = x.Id,
        OrderItemId = x.OrderItemId,
        ProductId = x.ProductId,
        ProductName = x.Product?.Name ?? string.Empty,
        WarehouseId = x.WarehouseId,
        WarehouseName = x.Warehouse?.Name ?? string.Empty,
        UnitOfMeasureId = x.UnitOfMeasureId,
        UnitOfMeasureName = x.UnitOfMeasure?.Name ?? string.Empty,
        InventoryStockId = x.InventoryStockId,
        ProductionOrderId = x.ProductionOrderId,
        LineNumber = x.LineNumber,
        OrderedQuantity = x.OrderedQuantity,
        AllocatedQuantity = x.AllocatedQuantity,
        PickedQuantity = x.PickedQuantity,
        PackedQuantity = x.PackedQuantity,
        ShippedQuantity = x.ShippedQuantity,
        DeliveredQuantity = x.DeliveredQuantity,
        ReturnedQuantity = x.ReturnedQuantity,
        UnitWeight = x.UnitWeight,
        UnitVolume = x.UnitVolume,
        LotNumber = x.LotNumber,
        SerialNumber = x.SerialNumber,
        ExpiryDateUtc = x.ExpiryDateUtc,
        Status = x.Status,
        Notes = x.Notes
    };

    private static ShipmentPackageResponse MapShipmentPackage(ShipmentPackage x) => new()
    {
        Id = x.Id,
        PackageNumber = x.PackageNumber,
        TrackingNumber = x.TrackingNumber,
        PackageType = x.PackageType,
        Length = x.Length,
        Width = x.Width,
        Height = x.Height,
        Weight = x.Weight,
        DeclaredValue = x.DeclaredValue,
        RequiresSpecialHandling = x.RequiresSpecialHandling,
        IsFragile = x.IsFragile,
        LabelUrl = x.LabelUrl,
        Barcode = x.Barcode,
        Status = x.Status,
        PackageItems = x.PackageItems.Select(MapShipmentPackageItem).ToList()
    };

    private static ShipmentPackageItemResponse MapShipmentPackageItem(ShipmentPackageItem x) => new()
    {
        Id = x.Id,
        ShipmentItemId = x.ShipmentItemId,
        LineNumber = x.ShipmentItem?.LineNumber ?? string.Empty,
        ProductId = x.ShipmentItem?.ProductId ?? Guid.Empty,
        ProductName = x.ShipmentItem?.Product?.Name ?? string.Empty,
        Quantity = x.Quantity
    };

    private static ShipmentTrackingEventResponse MapTrackingEvent(ShipmentTrackingEvent x) => new()
    {
        Id = x.Id,
        EventCode = x.EventCode,
        EventName = x.EventName,
        Description = x.Description,
        EventTimeUtc = x.EventTimeUtc,
        LocationName = x.LocationName,
        City = x.City,
        StateOrProvince = x.StateOrProvince,
        Country = x.Country,
        CarrierStatusCode = x.CarrierStatusCode,
        Source = x.Source,
        IsCustomerVisible = x.IsCustomerVisible
    };

    private static ShipmentDocumentResponse MapDocument(ShipmentDocument x) => new()
    {
        Id = x.Id,
        DocumentType = x.DocumentType,
        FileName = x.FileName,
        FileUrl = x.FileUrl,
        ContentType = x.ContentType,
        FileSizeBytes = x.FileSizeBytes,
        IsCustomerVisible = x.IsCustomerVisible,
        Notes = x.Notes
    };

    private static ShipmentChargeResponse MapCharge(ShipmentCharge x) => new()
    {
        Id = x.Id,
        ChargeType = x.ChargeType,
        Description = x.Description,
        Amount = x.Amount,
        CurrencyCode = x.CurrencyCode
    };

    private static ShipmentExceptionResponse MapException(ShipmentException x) => new()
    {
        Id = x.Id,
        ExceptionType = x.ExceptionType,
        Title = x.Title,
        Description = x.Description,
        ReportedAtUtc = x.ReportedAtUtc,
        ReportedBy = x.ReportedBy,
        IsResolved = x.IsResolved,
        ResolvedAtUtc = x.ResolvedAtUtc,
        ResolutionNote = x.ResolutionNote
    };

    private static ShipmentInsuranceResponse MapInsurance(ShipmentInsurance x) => new()
    {
        Id = x.Id,
        ProviderName = x.ProviderName,
        PolicyNumber = x.PolicyNumber,
        InsuredAmount = x.InsuredAmount,
        PremiumAmount = x.PremiumAmount,
        CurrencyCode = x.CurrencyCode,
        EffectiveDateUtc = x.EffectiveDateUtc,
        ExpiryDateUtc = x.ExpiryDateUtc,
        Status = x.Status,
        Notes = x.Notes
    };

    private static ShipmentStatusHistoryResponse MapStatusHistory(ShipmentStatusHistory x) => new()
    {
        Id = x.Id,
        FromStatus = x.FromStatus,
        ToStatus = x.ToStatus,
        ChangedAtUtc = x.ChangedAtUtc,
        ChangedBy = x.ChangedBy,
        Reason = x.Reason
    };

    private static ShipmentAddressResponse MapAddress(ShipmentAddress x) => new()
    {
        Id = x.Id,
        AddressType = x.AddressType,
        ContactName = x.ContactName,
        CompanyName = x.CompanyName,
        Phone = x.Phone,
        Email = x.Email,
        AddressLine1 = x.AddressLine1,
        AddressLine2 = x.AddressLine2,
        City = x.City,
        StateOrProvince = x.StateOrProvince,
        PostalCode = x.PostalCode,
        Country = x.Country
    };

    private static DeliveryRunResponse MapDeliveryRun(DeliveryRun x) => new()
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

    private static DockAppointmentResponse MapDockAppointment(DockAppointment x) => new()
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

    private static CustomsDocumentResponse MapCustomsDocument(CustomsDocument x) => new()
    {
        Id = x.Id,
        DocumentType = x.DocumentType,
        DocumentNumber = x.DocumentNumber,
        FileName = x.FileName,
        FileUrl = x.FileUrl,
        CountryOfOrigin = x.CountryOfOrigin,
        DestinationCountry = x.DestinationCountry,
        HarmonizedCode = x.HarmonizedCode,
        DeclaredCustomsValue = x.DeclaredCustomsValue,
        CurrencyCode = x.CurrencyCode,
        IssuedAtUtc = x.IssuedAtUtc,
        Notes = x.Notes
    };

    private static ReturnShipmentResponse MapReturnShipment(ReturnShipment x) => new()
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
        Items = x.Items.Select(i => new ReturnShipmentItemResponse
        {
            Id = i.Id,
            ShipmentItemId = i.ShipmentItemId,
            ProductId = i.ShipmentItem?.ProductId ?? Guid.Empty,
            ProductName = i.ShipmentItem?.Product?.Name ?? string.Empty,
            ReturnedQuantity = i.ReturnedQuantity,
            ReturnCondition = i.ReturnCondition,
            InspectionResult = i.InspectionResult,
            Notes = i.Notes
        }).ToList()
    };
}
