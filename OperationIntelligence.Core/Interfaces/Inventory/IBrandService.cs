namespace OperationIntelligence.Core;

public interface IBrandService
{
    Task<BrandResponse> CreateAsync(CreateBrandRequest request, CancellationToken cancellationToken = default);
    Task<BrandResponse?> UpdateAsync(UpdateBrandRequest request, CancellationToken cancellationToken = default);
    Task<BrandResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BrandResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
