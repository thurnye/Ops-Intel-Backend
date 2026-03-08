using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<BrandResponse> CreateAsync(CreateBrandRequest request, CancellationToken cancellationToken = default)
    {
        var existingByName = await _brandRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingByName != null)
            throw new InvalidOperationException(InventoryErrorMessages.BrandAlreadyExists(request.Name));

        var brand = new Brand
        {
            Name = request.Name,
            Description = request.Description
        };

        await _brandRepository.AddAsync(brand, cancellationToken);
        await _brandRepository.SaveChangesAsync(cancellationToken);

        return Map(brand);
    }

    public async Task<BrandResponse?> UpdateAsync(UpdateBrandRequest request, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (brand == null)
            return null;

        var existingByName = await _brandRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingByName != null && existingByName.Id != request.Id)
            throw new InvalidOperationException(InventoryErrorMessages.BrandAlreadyExists(request.Name));

        brand.Name = request.Name;
        brand.Description = request.Description;
        brand.UpdatedAtUtc = DateTime.UtcNow;

        _brandRepository.Update(brand);
        await _brandRepository.SaveChangesAsync(cancellationToken);

        return Map(brand);
    }

    public async Task<BrandResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetByIdAsync(id, cancellationToken);
        return brand == null ? null : Map(brand);
    }

    public async Task<IReadOnlyList<BrandResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var brands = await _brandRepository.GetAllAsync(cancellationToken);
        return brands.Select(Map).ToList();
    }

    private static BrandResponse Map(Brand brand) => new()
    {
        Id = brand.Id,
        Name = brand.Name,
        Description = brand.Description
    };
}
