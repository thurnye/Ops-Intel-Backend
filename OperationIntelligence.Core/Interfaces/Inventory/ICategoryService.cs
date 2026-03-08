namespace OperationIntelligence.Core;

public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
