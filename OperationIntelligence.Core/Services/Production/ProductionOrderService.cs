using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductionOrderService : IProductionOrderService
{
    private readonly IProductionOrderRepository _productionOrderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfMeasureRepository _uomRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IBillOfMaterialRepository _bomRepository;
    private readonly IRoutingRepository _routingRepository;

    public ProductionOrderService(
        IProductionOrderRepository productionOrderRepository,
        IProductRepository productRepository,
        IUnitOfMeasureRepository uomRepository,
        IWarehouseRepository warehouseRepository,
        IBillOfMaterialRepository bomRepository,
        IRoutingRepository routingRepository)
    {
        _productionOrderRepository = productionOrderRepository;
        _productRepository = productRepository;
        _uomRepository = uomRepository;
        _warehouseRepository = warehouseRepository;
        _bomRepository = bomRepository;
        _routingRepository = routingRepository;
    }

    public async Task<ProductionOrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _productionOrderRepository.GetWithDetailsAsync(id, cancellationToken);
        return entity is null || entity.IsDeleted ? null : entity.ToResponse();
    }

    public async Task<PagedResponse<ProductionOrderSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var query = _productionOrderRepository.Query().AsNoTracking().Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedAtUtc);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => new ProductionOrderSummaryResponse
        {
            Id = x.Id,
            ProductionOrderNumber = x.ProductionOrderNumber,
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            ProductSku = x.Product.SKU,
            PlannedQuantity = x.PlannedQuantity,
            ProducedQuantity = x.ProducedQuantity,
            RemainingQuantity = x.RemainingQuantity,
            UnitOfMeasureId = x.UnitOfMeasureId,
            UnitOfMeasureName = x.UnitOfMeasure.Name,
            WarehouseId = x.WarehouseId,
            WarehouseName = x.Warehouse.Name,
            PlannedStartDate = x.PlannedStartDate,
            PlannedEndDate = x.PlannedEndDate,
            Status = x.Status,
            Priority = x.Priority,
            IsReleased = x.IsReleased,
            IsClosed = x.IsClosed,
            CreatedAtUtc = x.CreatedAtUtc
        }).ToListAsync(cancellationToken);

        return new PagedResponse<ProductionOrderSummaryResponse> { PageNumber = pageNumber, PageSize = pageSize, TotalRecords = total, Items = items };
    }

    public async Task<ProductionOrderResponse> CreateAsync(CreateProductionOrderRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var productExists = await _productRepository.ExistsAsync(x => x.Id == request.ProductId && !x.IsDeleted, cancellationToken);
        if (!productExists) throw new InvalidOperationException(ProductionErrorMessages.ProductDoesNotExist);

        var uomExists = await _uomRepository.ExistsAsync(x => x.Id == request.UnitOfMeasureId && !x.IsDeleted, cancellationToken);
        if (!uomExists) throw new InvalidOperationException(ProductionErrorMessages.UnitOfMeasureDoesNotExist);

        var warehouseExists = await _warehouseRepository.ExistsAsync(x => x.Id == request.WarehouseId && !x.IsDeleted, cancellationToken);
        if (!warehouseExists) throw new InvalidOperationException(ProductionErrorMessages.WarehouseDoesNotExist);

        if (request.BillOfMaterialId.HasValue)
        {
            var bom = await _bomRepository.GetByIdAsync(request.BillOfMaterialId.Value, cancellationToken);
            if (bom is null || bom.IsDeleted || !bom.IsActive) throw new InvalidOperationException(ProductionErrorMessages.BillOfMaterialDoesNotExistOrIsInactive);
        }

        if (request.RoutingId.HasValue)
        {
            var routing = await _routingRepository.GetByIdAsync(request.RoutingId.Value, cancellationToken);
            if (routing is null || routing.IsDeleted || !routing.IsActive) throw new InvalidOperationException(ProductionErrorMessages.RoutingDoesNotExistOrIsInactive);
        }

        var exists = await _productionOrderRepository.ProductionOrderNumberExistsAsync(request.ProductionOrderNumber.Trim(), null, cancellationToken);
        if (exists) throw new InvalidOperationException(ProductionErrorMessages.ProductionOrderNumberAlreadyExists);

        var entity = new ProductionOrder
        {
            ProductionOrderNumber = request.ProductionOrderNumber.Trim(),
            ProductId = request.ProductId,
            PlannedQuantity = request.PlannedQuantity,
            ProducedQuantity = 0,
            ScrapQuantity = 0,
            RemainingQuantity = request.PlannedQuantity,
            UnitOfMeasureId = request.UnitOfMeasureId,
            BillOfMaterialId = request.BillOfMaterialId,
            RoutingId = request.RoutingId,
            WarehouseId = request.WarehouseId,
            PlannedStartDate = request.PlannedStartDate,
            PlannedEndDate = request.PlannedEndDate,
            Priority = request.Priority,
            SourceType = request.SourceType,
            SourceReferenceId = request.SourceReferenceId,
            BatchNumber = request.BatchNumber?.Trim(),
            LotNumber = request.LotNumber?.Trim(),
            Notes = request.Notes?.Trim(),
            Status = ProductionOrderStatus.Draft,
            IsReleased = false,
            IsClosed = false,
            CreatedBy = createdBy
        };

        await _productionOrderRepository.AddAsync(entity, cancellationToken);
        await _productionOrderRepository.SaveChangesAsync(cancellationToken);

        var created = await _productionOrderRepository.GetWithDetailsAsync(entity.Id, cancellationToken) ?? entity;
        return created.ToResponse();
    }

    public async Task<ProductionOrderResponse?> UpdateAsync(Guid id, UpdateProductionOrderRequest request, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _productionOrderRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return null;

        entity.ProductId = request.ProductId;
        entity.PlannedQuantity = request.PlannedQuantity;
        entity.RemainingQuantity = Math.Max(0, request.PlannedQuantity - entity.ProducedQuantity - entity.ScrapQuantity);
        entity.UnitOfMeasureId = request.UnitOfMeasureId;
        entity.BillOfMaterialId = request.BillOfMaterialId;
        entity.RoutingId = request.RoutingId;
        entity.WarehouseId = request.WarehouseId;
        entity.PlannedStartDate = request.PlannedStartDate;
        entity.PlannedEndDate = request.PlannedEndDate;
        entity.Priority = request.Priority;
        entity.BatchNumber = request.BatchNumber?.Trim();
        entity.LotNumber = request.LotNumber?.Trim();
        entity.Notes = request.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;

        _productionOrderRepository.Update(entity);
        await _productionOrderRepository.SaveChangesAsync(cancellationToken);

        var updated = await _productionOrderRepository.GetWithDetailsAsync(entity.Id, cancellationToken) ?? entity;
        return updated.ToResponse();
    }

    public Task<bool> ReleaseAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default) =>
        ChangeStatusAsync(id, ProductionOrderStatus.Released, updatedBy, cancellationToken, markReleased: true);

    public Task<bool> StartAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default) =>
        ChangeStatusAsync(id, ProductionOrderStatus.InProgress, updatedBy, cancellationToken, setActualStart: true);

    public Task<bool> CompleteAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default) =>
        ChangeStatusAsync(id, ProductionOrderStatus.Completed, updatedBy, cancellationToken, setActualEnd: true, setRemainingZero: true);

    public Task<bool> CloseAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default) =>
        ChangeStatusAsync(id, ProductionOrderStatus.Closed, updatedBy, cancellationToken, markClosed: true);

    public Task<bool> CancelAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default) =>
        ChangeStatusAsync(id, ProductionOrderStatus.Cancelled, updatedBy, cancellationToken);

    private async Task<bool> ChangeStatusAsync(
        Guid id,
        ProductionOrderStatus status,
        string? updatedBy,
        CancellationToken cancellationToken,
        bool markReleased = false,
        bool markClosed = false,
        bool setActualStart = false,
        bool setActualEnd = false,
        bool setRemainingZero = false)
    {
        var entity = await _productionOrderRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.Status = status;
        entity.IsReleased = markReleased || entity.IsReleased;
        entity.IsClosed = markClosed || entity.IsClosed;
        entity.ActualStartDate = setActualStart ? DateTime.UtcNow : entity.ActualStartDate;
        entity.ActualEndDate = setActualEnd ? DateTime.UtcNow : entity.ActualEndDate;
        entity.RemainingQuantity = setRemainingZero ? 0 : entity.RemainingQuantity;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;

        _productionOrderRepository.Update(entity);
        await _productionOrderRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
