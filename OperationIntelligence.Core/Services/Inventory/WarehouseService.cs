using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<WarehouseResponse> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        var nameExists = await _warehouseRepository.GetByNameAsync(request.Name, cancellationToken);
        if (nameExists != null)
            throw new InvalidOperationException(InventoryErrorMessages.WarehouseAlreadyExists(request.Name));

        var codeExists = await _warehouseRepository.ExistsAsync(x => x.Code == request.Code, cancellationToken);
        if (codeExists)
            throw new InvalidOperationException(InventoryErrorMessages.WarehouseCodeAlreadyExists(request.Code));

        var warehouse = new Warehouse
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            StateOrProvince = request.StateOrProvince,
            PostalCode = request.PostalCode,
            Country = request.Country,
            IsActive = request.IsActive
        };

        await _warehouseRepository.AddAsync(warehouse, cancellationToken);
        await _warehouseRepository.SaveChangesAsync(cancellationToken);

        return Map(warehouse);
    }

    public async Task<WarehouseResponse?> UpdateAsync(UpdateWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (warehouse == null)
            return null;

        var nameExists = await _warehouseRepository.GetByNameAsync(request.Name, cancellationToken);
        if (nameExists != null && nameExists.Id != request.Id)
            throw new InvalidOperationException(InventoryErrorMessages.WarehouseAlreadyExists(request.Name));

        var codeExists = await _warehouseRepository.ExistsAsync(x => x.Code == request.Code && x.Id != request.Id, cancellationToken);
        if (codeExists)
            throw new InvalidOperationException(InventoryErrorMessages.WarehouseCodeAlreadyExists(request.Code));

        warehouse.Name = request.Name;
        warehouse.Code = request.Code;
        warehouse.Description = request.Description;
        warehouse.AddressLine1 = request.AddressLine1;
        warehouse.AddressLine2 = request.AddressLine2;
        warehouse.City = request.City;
        warehouse.StateOrProvince = request.StateOrProvince;
        warehouse.PostalCode = request.PostalCode;
        warehouse.Country = request.Country;
        warehouse.IsActive = request.IsActive;
        warehouse.UpdatedAtUtc = DateTime.UtcNow;

        _warehouseRepository.Update(warehouse);
        await _warehouseRepository.SaveChangesAsync(cancellationToken);

        return Map(warehouse);
    }

    public async Task<WarehouseResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id, cancellationToken);
        return warehouse == null ? null : Map(warehouse);
    }

    public async Task<IReadOnlyList<WarehouseResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var warehouses = await _warehouseRepository.GetAllAsync(cancellationToken);
        return warehouses.Select(Map).ToList();
    }

    public async Task<WarehouseMetricsSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var warehouses = await _warehouseRepository.GetAllAsync(cancellationToken);

        return new WarehouseMetricsSummaryResponse
        {
            TotalWarehouses = warehouses.Count,
            ActiveWarehouses = warehouses.Count(warehouse => warehouse.IsActive),
            CountriesRepresented = warehouses
                .Where(warehouse => !string.IsNullOrWhiteSpace(warehouse.Country))
                .Select(warehouse => warehouse.Country!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count(),
            AddressReadyWarehouses = warehouses.Count(warehouse =>
                !string.IsNullOrWhiteSpace(warehouse.AddressLine1) &&
                !string.IsNullOrWhiteSpace(warehouse.City) &&
                !string.IsNullOrWhiteSpace(warehouse.Country))
        };
    }

    private static WarehouseResponse Map(Warehouse warehouse) => new()
    {
        Id = warehouse.Id,
        Name = warehouse.Name,
        Code = warehouse.Code,
        Description = warehouse.Description,
        AddressLine1 = warehouse.AddressLine1,
        AddressLine2 = warehouse.AddressLine2,
        City = warehouse.City,
        StateOrProvince = warehouse.StateOrProvince,
        PostalCode = warehouse.PostalCode,
        Country = warehouse.Country,
        IsActive = warehouse.IsActive
    };
}
