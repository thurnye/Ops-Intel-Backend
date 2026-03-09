using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class BillOfMaterialService : IBillOfMaterialService
{
    private readonly IBillOfMaterialRepository _bomRepository;
    private readonly IBillOfMaterialItemRepository _bomItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfMeasureRepository _uomRepository;

    public BillOfMaterialService(
        IBillOfMaterialRepository bomRepository,
        IBillOfMaterialItemRepository bomItemRepository,
        IProductRepository productRepository,
        IUnitOfMeasureRepository uomRepository)
    {
        _bomRepository = bomRepository;
        _bomItemRepository = bomItemRepository;
        _productRepository = productRepository;
        _uomRepository = uomRepository;
    }

    public async Task<BillOfMaterialResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _bomRepository.GetWithItemsAsync(id, cancellationToken);
        return entity is null || entity.IsDeleted ? null : entity.ToResponse();
    }

    public async Task<PagedResponse<BillOfMaterialSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var query = _bomRepository.Query().AsNoTracking().Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedAtUtc);
        var total = await query.CountAsync(cancellationToken);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => new BillOfMaterialSummaryResponse
        {
            Id = x.Id,
            BomCode = x.BomCode,
            Name = x.Name,
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            ProductSku = x.Product.SKU,
            BaseQuantity = x.BaseQuantity,
            UnitOfMeasureName = x.UnitOfMeasure.Name,
            Version = x.Version,
            IsActive = x.IsActive,
            IsDefault = x.IsDefault,
            EffectiveFrom = x.EffectiveFrom,
            EffectiveTo = x.EffectiveTo
        }).ToListAsync(cancellationToken);

        return new PagedResponse<BillOfMaterialSummaryResponse> { PageNumber = pageNumber, PageSize = pageSize, TotalRecords = total, Items = items };
    }

    public async Task<BillOfMaterialResponse> CreateAsync(CreateBillOfMaterialRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var productExists = await _productRepository.ExistsAsync(x => x.Id == request.ProductId && !x.IsDeleted, cancellationToken);
        if (!productExists) throw new InvalidOperationException("Product does not exist.");

        var uomExists = await _uomRepository.ExistsAsync(x => x.Id == request.UnitOfMeasureId && !x.IsDeleted, cancellationToken);
        if (!uomExists) throw new InvalidOperationException("Unit of measure does not exist.");

        var codeExists = await _bomRepository.BomCodeExistsAsync(request.BomCode.Trim(), null, cancellationToken);
        if (codeExists) throw new InvalidOperationException("BOM code already exists.");

        var entity = new BillOfMaterial
        {
            BomCode = request.BomCode.Trim(),
            Name = request.Name.Trim(),
            ProductId = request.ProductId,
            BaseQuantity = request.BaseQuantity,
            UnitOfMeasureId = request.UnitOfMeasureId,
            Version = request.Version,
            IsActive = request.IsActive,
            IsDefault = request.IsDefault,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _bomRepository.AddAsync(entity, cancellationToken);
        await _bomRepository.SaveChangesAsync(cancellationToken);

        var created = await _bomRepository.GetWithItemsAsync(entity.Id, cancellationToken) ?? entity;
        return created.ToResponse();
    }

    public async Task<BillOfMaterialItemResponse> AddItemAsync(CreateBillOfMaterialItemRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var bom = await _bomRepository.GetByIdAsync(request.BillOfMaterialId, cancellationToken);
        if (bom is null || bom.IsDeleted) throw new InvalidOperationException("BOM does not exist.");

        var productExists = await _productRepository.ExistsAsync(x => x.Id == request.MaterialProductId && !x.IsDeleted, cancellationToken);
        if (!productExists) throw new InvalidOperationException("Material product does not exist.");

        var uomExists = await _uomRepository.ExistsAsync(x => x.Id == request.UnitOfMeasureId && !x.IsDeleted, cancellationToken);
        if (!uomExists) throw new InvalidOperationException("Unit of measure does not exist.");

        var sequenceExists = await _bomItemRepository.GetByBillOfMaterialAndSequenceAsync(request.BillOfMaterialId, request.Sequence, cancellationToken);
        if (sequenceExists is not null) throw new InvalidOperationException("Sequence already exists in this BOM.");

        var entity = new BillOfMaterialItem
        {
            BillOfMaterialId = request.BillOfMaterialId,
            MaterialProductId = request.MaterialProductId,
            QuantityRequired = request.QuantityRequired,
            UnitOfMeasureId = request.UnitOfMeasureId,
            ScrapFactorPercent = request.ScrapFactorPercent,
            YieldFactorPercent = request.YieldFactorPercent,
            IsOptional = request.IsOptional,
            IsBackflush = request.IsBackflush,
            Sequence = request.Sequence,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _bomItemRepository.AddAsync(entity, cancellationToken);
        await _bomItemRepository.SaveChangesAsync(cancellationToken);

        var createdItems = await _bomItemRepository.GetByBillOfMaterialIdAsync(request.BillOfMaterialId, cancellationToken);
        var created = createdItems.First(x => x.Id == entity.Id);
        return created.ToResponse();
    }

    public async Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _bomRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;

        _bomRepository.Update(entity);
        await _bomRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
