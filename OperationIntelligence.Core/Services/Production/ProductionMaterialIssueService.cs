using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductionMaterialIssueService : IProductionMaterialIssueService
{
    private readonly IProductionMaterialIssueRepository _issueRepository;
    private readonly IProductionOrderRepository _orderRepository;

    public ProductionMaterialIssueService(IProductionMaterialIssueRepository issueRepository, IProductionOrderRepository orderRepository)
    {
        _issueRepository = issueRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<ProductionMaterialIssueResponse>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        var items = await _issueRepository.GetByProductionOrderIdAsync(productionOrderId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ProductionMaterialIssueResponse> CreateAsync(CreateProductionMaterialIssueRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var orderExists = await _orderRepository.ExistsAsync(x => x.Id == request.ProductionOrderId && !x.IsDeleted, cancellationToken);
        if (!orderExists) throw new InvalidOperationException(ProductionErrorMessages.ProductionOrderDoesNotExist);

        var entity = new ProductionMaterialIssue
        {
            ProductionOrderId = request.ProductionOrderId,
            MaterialProductId = request.MaterialProductId,
            WarehouseId = request.WarehouseId,
            PlannedQuantity = request.PlannedQuantity,
            IssuedQuantity = request.IssuedQuantity,
            ReturnedQuantity = 0,
            UnitOfMeasureId = request.UnitOfMeasureId,
            BatchNumber = request.BatchNumber?.Trim(),
            LotNumber = request.LotNumber?.Trim(),
            IssueDate = request.IssueDate,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _issueRepository.AddAsync(entity, cancellationToken);
        await _issueRepository.SaveChangesAsync(cancellationToken);

        var created = await _issueRepository.GetWithConsumptionsAsync(entity.Id, cancellationToken) ?? entity;
        return created.ToResponse();
    }
}
