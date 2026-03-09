using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class BillOfMaterialItemService : IBillOfMaterialItemService
{
    private readonly IBillOfMaterialItemRepository _bomItemRepository;

    public BillOfMaterialItemService(IBillOfMaterialItemRepository bomItemRepository)
    {
        _bomItemRepository = bomItemRepository;
    }

    public async Task<IReadOnlyList<BillOfMaterialItemResponse>> GetByBillOfMaterialIdAsync(Guid billOfMaterialId, CancellationToken cancellationToken = default)
    {
        var items = await _bomItemRepository.GetByBillOfMaterialIdAsync(billOfMaterialId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<BillOfMaterialItemResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _bomItemRepository.GetByIdAsync(id, cancellationToken);
        return entity is null || entity.IsDeleted ? null : entity.ToResponse();
    }

    public async Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _bomItemRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;

        _bomItemRepository.Update(entity);
        await _bomItemRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
