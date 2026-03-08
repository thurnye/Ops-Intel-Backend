using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var existingByName = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingByName != null)
            throw new InvalidOperationException(InventoryErrorMessages.CategoryAlreadyExists(request.Name));

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            return null;

        var existingByName = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingByName != null && existingByName.Id != request.Id)
            throw new InvalidOperationException(InventoryErrorMessages.CategoryAlreadyExists(request.Name));

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;
        category.UpdatedAtUtc = DateTime.UtcNow;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category == null ? null : Map(category);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(Map).ToList();
    }

    private static CategoryResponse Map(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        ParentCategoryId = category.ParentCategoryId
    };
}
