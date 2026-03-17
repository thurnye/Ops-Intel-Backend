using FluentValidation;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentCarrierService : ICarrierService
{
    private readonly ICarrierRepository _carrierRepository;
    private readonly IValidator<CreateCarrierRequest> _createCarrierValidator;
    private readonly IValidator<UpdateCarrierRequest> _updateCarrierValidator;
    private readonly IValidator<CreateCarrierServiceRequest> _createCarrierServiceValidator;
    private readonly IValidator<UpdateCarrierServiceRequest> _updateCarrierServiceValidator;

    public ShipmentCarrierService(
        ICarrierRepository carrierRepository,
        IValidator<CreateCarrierRequest> createCarrierValidator,
        IValidator<UpdateCarrierRequest> updateCarrierValidator,
        IValidator<CreateCarrierServiceRequest> createCarrierServiceValidator,
        IValidator<UpdateCarrierServiceRequest> updateCarrierServiceValidator)
    {
        _carrierRepository = carrierRepository;
        _createCarrierValidator = createCarrierValidator;
        _updateCarrierValidator = updateCarrierValidator;
        _createCarrierServiceValidator = createCarrierServiceValidator;
        _updateCarrierServiceValidator = updateCarrierServiceValidator;
    }

    public async Task<PagedResponse<CarrierResponse>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var items = await _carrierRepository.GetPagedAsync(pageNumber, pageSize, search, isActive, cancellationToken);
        var total = await _carrierRepository.CountAsync(search, isActive, cancellationToken);

        return new PagedResponse<CarrierResponse>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = total,
            Items = items.Select(MapCarrier).ToList()
        };
    }

    public async Task<CarrierMetricsSummaryResponse> GetSummaryAsync(string? search = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var carriers = await _carrierRepository.GetPagedAsync(1, int.MaxValue, search, isActive, cancellationToken);

        return new CarrierMetricsSummaryResponse
        {
            TotalCarriers = carriers.Count,
            ActiveCarriers = carriers.Count(carrier => carrier.IsActive),
            ContactableCarriers = carriers.Count(carrier =>
                !string.IsNullOrWhiteSpace(carrier.Email) ||
                !string.IsNullOrWhiteSpace(carrier.Phone)),
            TotalServices = carriers.Sum(carrier => carrier.Services.Count(service => !service.IsDeleted))
        };
    }

    public async Task<CarrierResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var carrier = await _carrierRepository.GetByIdWithServicesAsync(id, cancellationToken);
        return carrier == null ? null : MapCarrier(carrier);
    }

    public async Task<CarrierResponse> CreateAsync(CreateCarrierRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _createCarrierValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (await _carrierRepository.ExistsAsync(x => x.CarrierCode == request.CarrierCode, cancellationToken))
            throw new InvalidOperationException("Carrier code already exists.");

        var carrier = new Carrier
        {
            CarrierCode = request.CarrierCode,
            Name = request.Name,
            ContactName = request.ContactName,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
            IsActive = request.IsActive,
            CreatedBy = currentUser
        };

        await _carrierRepository.AddAsync(carrier, cancellationToken);
        await _carrierRepository.SaveChangesAsync(cancellationToken);

        var created = await _carrierRepository.GetByIdWithServicesAsync(carrier.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created carrier could not be reloaded.");

        return MapCarrier(created);
    }

    public Task<BulkCreateResponse<CarrierResponse>> CreateBulkAsync(
        BulkCreateRequest<CreateCarrierRequest> request,
        string? currentUser = null,
        CancellationToken cancellationToken = default) =>
        BulkCreateExecutor.ExecuteAsync(
            request.Items,
            (item, token) => CreateAsync(item, currentUser, token),
            cancellationToken);

    public async Task<CarrierResponse> UpdateAsync(Guid id, UpdateCarrierRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _updateCarrierValidator.ValidateAndThrowAsync(request, cancellationToken);

        var carrier = await _carrierRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Carrier not found.");

        carrier.Name = request.Name;
        carrier.ContactName = request.ContactName;
        carrier.Phone = request.Phone;
        carrier.Email = request.Email;
        carrier.Website = request.Website;
        carrier.IsActive = request.IsActive;
        carrier.UpdatedAtUtc = DateTime.UtcNow;
        carrier.UpdatedBy = currentUser;

        _carrierRepository.Update(carrier);
        await _carrierRepository.SaveChangesAsync(cancellationToken);

        var updated = await _carrierRepository.GetByIdWithServicesAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Updated carrier could not be reloaded.");

        return MapCarrier(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        var carrier = await _carrierRepository.GetByIdAsync(id, cancellationToken);
        if (carrier == null) return false;

        carrier.IsDeleted = true;
        carrier.DeletedAtUtc = DateTime.UtcNow;
        carrier.DeletedBy = currentUser;
        carrier.UpdatedAtUtc = DateTime.UtcNow;
        carrier.UpdatedBy = currentUser;

        _carrierRepository.Update(carrier);
        await _carrierRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<CarrierServiceResponse> CreateServiceAsync(CreateCarrierServiceRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _createCarrierServiceValidator.ValidateAndThrowAsync(request, cancellationToken);

        var carrier = await _carrierRepository.GetByIdAsync(request.CarrierId, cancellationToken)
            ?? throw new KeyNotFoundException("Carrier not found.");

        var exists = await _carrierRepository.GetServiceByCodeAsync(request.CarrierId, request.ServiceCode, cancellationToken);
        if (exists != null)
            throw new InvalidOperationException("Carrier service code already exists for this carrier.");

        var service = new CarrierService
        {
            CarrierId = request.CarrierId,
            ServiceCode = request.ServiceCode,
            Name = request.Name,
            Description = request.Description,
            EstimatedTransitDays = request.EstimatedTransitDays,
            IsActive = request.IsActive,
            CreatedBy = currentUser
        };

        await _carrierRepository.AddServiceAsync(service, cancellationToken);
        await _carrierRepository.SaveChangesAsync(cancellationToken);

        var created = await _carrierRepository.GetServiceByIdAsync(service.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Created carrier service could not be reloaded.");

        return MapCarrierService(created);
    }

    public async Task<CarrierServiceResponse> UpdateServiceAsync(Guid id, UpdateCarrierServiceRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _updateCarrierServiceValidator.ValidateAndThrowAsync(request, cancellationToken);

        var service = await _carrierRepository.GetServiceByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Carrier service not found.");

        service.Name = request.Name;
        service.Description = request.Description;
        service.EstimatedTransitDays = request.EstimatedTransitDays;
        service.IsActive = request.IsActive;
        service.UpdatedAtUtc = DateTime.UtcNow;
        service.UpdatedBy = currentUser;

        _carrierRepository.UpdateService(service);
        await _carrierRepository.SaveChangesAsync(cancellationToken);

        var updated = await _carrierRepository.GetServiceByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Updated carrier service could not be reloaded.");

        return MapCarrierService(updated);
    }

    public async Task<bool> DeleteServiceAsync(Guid id, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        var service = await _carrierRepository.GetServiceByIdAsync(id, cancellationToken);
        if (service == null) return false;

        service.IsDeleted = true;
        service.DeletedAtUtc = DateTime.UtcNow;
        service.DeletedBy = currentUser;
        service.UpdatedAtUtc = DateTime.UtcNow;
        service.UpdatedBy = currentUser;

        _carrierRepository.UpdateService(service);
        await _carrierRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<CarrierServiceResponse>> GetServicesByCarrierIdAsync(Guid carrierId, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        if (!await _carrierRepository.ExistsAsync(x => x.Id == carrierId, cancellationToken))
            throw new KeyNotFoundException("Carrier not found.");

        var services = await _carrierRepository.GetServicesByCarrierIdAsync(carrierId, isActive, cancellationToken);
        return services.Select(MapCarrierService).ToList();
    }

    private static CarrierResponse MapCarrier(Carrier x) => new()
    {
        Id = x.Id,
        CarrierCode = x.CarrierCode,
        Name = x.Name,
        ContactName = x.ContactName,
        Phone = x.Phone,
        Email = x.Email,
        Website = x.Website,
        IsActive = x.IsActive,
        Services = x.Services.Select(MapCarrierService).ToList()
    };

    private static CarrierServiceResponse MapCarrierService(CarrierService x) => new()
    {
        Id = x.Id,
        CarrierId = x.CarrierId,
        ServiceCode = x.ServiceCode,
        Name = x.Name,
        Description = x.Description,
        EstimatedTransitDays = x.EstimatedTransitDays,
        IsActive = x.IsActive
    };
}
