using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierResponse> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var nameExists = await _supplierRepository.GetByNameAsync(request.Name, cancellationToken);
        if (nameExists != null)
            throw new InvalidOperationException(InventoryErrorMessages.SupplierAlreadyExists(request.Name));

        var supplier = new Supplier
        {
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            StateOrProvince = request.StateOrProvince,
            PostalCode = request.PostalCode,
            Country = request.Country,
            IsActive = request.IsActive,
            Notes = request.Notes
        };

        await _supplierRepository.AddAsync(supplier, cancellationToken);
        await _supplierRepository.SaveChangesAsync(cancellationToken);

        return Map(supplier);
    }

    public Task<BulkCreateResponse<SupplierResponse>> CreateBulkAsync(
        BulkCreateRequest<CreateSupplierRequest> request,
        CancellationToken cancellationToken = default) =>
        BulkCreateExecutor.ExecuteAsync(
            request.Items,
            (item, token) => CreateAsync(item, token),
            cancellationToken);

    public async Task<SupplierResponse?> UpdateAsync(UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier == null)
            return null;

        var nameExists = await _supplierRepository.GetByNameAsync(request.Name, cancellationToken);
        if (nameExists != null && nameExists.Id != request.Id)
            throw new InvalidOperationException(InventoryErrorMessages.SupplierAlreadyExists(request.Name));

        supplier.Name = request.Name;
        supplier.ContactPerson = request.ContactPerson;
        supplier.Email = request.Email;
        supplier.PhoneNumber = request.PhoneNumber;
        supplier.AddressLine1 = request.AddressLine1;
        supplier.AddressLine2 = request.AddressLine2;
        supplier.City = request.City;
        supplier.StateOrProvince = request.StateOrProvince;
        supplier.PostalCode = request.PostalCode;
        supplier.Country = request.Country;
        supplier.IsActive = request.IsActive;
        supplier.Notes = request.Notes;
        supplier.UpdatedAtUtc = DateTime.UtcNow;

        _supplierRepository.Update(supplier);
        await _supplierRepository.SaveChangesAsync(cancellationToken);

        return Map(supplier);
    }

    public async Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        return supplier == null ? null : Map(supplier);
    }

    public async Task<IReadOnlyList<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        return suppliers.Select(Map).ToList();
    }

    public async Task<SupplierMetricsSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);

        return new SupplierMetricsSummaryResponse
        {
            TotalSuppliers = suppliers.Count,
            ActiveSuppliers = suppliers.Count(supplier => supplier.IsActive),
            ContactableSuppliers = suppliers.Count(supplier =>
                !string.IsNullOrWhiteSpace(supplier.Email) ||
                !string.IsNullOrWhiteSpace(supplier.PhoneNumber)),
            CountriesRepresented = suppliers
                .Where(supplier => !string.IsNullOrWhiteSpace(supplier.Country))
                .Select(supplier => supplier.Country!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count()
        };
    }

    private static SupplierResponse Map(Supplier supplier) => new()
    {
        Id = supplier.Id,
        Name = supplier.Name,
        ContactPerson = supplier.ContactPerson,
        Email = supplier.Email,
        PhoneNumber = supplier.PhoneNumber,
        AddressLine1 = supplier.AddressLine1,
        AddressLine2 = supplier.AddressLine2,
        City = supplier.City,
        StateOrProvince = supplier.StateOrProvince,
        PostalCode = supplier.PostalCode,
        Country = supplier.Country,
        IsActive = supplier.IsActive,
        Notes = supplier.Notes
    };
}
