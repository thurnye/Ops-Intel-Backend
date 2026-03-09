using FluentValidation;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentAddressService : IShipmentAddressService
{
    private readonly IShipmentAddressRepository _addressRepository;
    private readonly IValidator<CreateShipmentAddressRequest> _createValidator;
    private readonly IValidator<UpdateShipmentAddressRequest> _updateValidator;

    public ShipmentAddressService(
        IShipmentAddressRepository addressRepository,
        IValidator<CreateShipmentAddressRequest> createValidator,
        IValidator<UpdateShipmentAddressRequest> updateValidator)
    {
        _addressRepository = addressRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ShipmentAddressResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var address = await _addressRepository.GetByIdAsync(id, cancellationToken);
        return address == null ? null : Map(address);
    }

    public async Task<IReadOnlyList<ShipmentAddressResponse>> SearchAsync(string? search = null, string? country = null, string? city = null, int take = 25, CancellationToken cancellationToken = default)
    {
        var items = await _addressRepository.SearchAsync(search, country, city, take, cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<ShipmentAddressResponse> CreateAsync(CreateShipmentAddressRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = new ShipmentAddress
        {
            AddressType = request.AddressType,
            ContactName = request.ContactName,
            CompanyName = request.CompanyName,
            Phone = request.Phone,
            Email = request.Email,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            StateOrProvince = request.StateOrProvince,
            PostalCode = request.PostalCode,
            Country = request.Country,
            CreatedBy = currentUser
        };

        await _addressRepository.AddAsync(entity, cancellationToken);
        await _addressRepository.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<ShipmentAddressResponse> UpdateAsync(Guid id, UpdateShipmentAddressRequest request, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _addressRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Shipment address not found.");

        entity.AddressType = request.AddressType;
        entity.ContactName = request.ContactName;
        entity.CompanyName = request.CompanyName;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.AddressLine1 = request.AddressLine1;
        entity.AddressLine2 = request.AddressLine2;
        entity.City = request.City;
        entity.StateOrProvince = request.StateOrProvince;
        entity.PostalCode = request.PostalCode;
        entity.Country = request.Country;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = currentUser;

        _addressRepository.Update(entity);
        await _addressRepository.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, string? currentUser = null, CancellationToken cancellationToken = default)
    {
        var entity = await _addressRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = currentUser;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = currentUser;

        _addressRepository.Update(entity);
        await _addressRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ShipmentAddressResponse Map(ShipmentAddress x) => new()
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
}