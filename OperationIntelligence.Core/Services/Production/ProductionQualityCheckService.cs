using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductionQualityCheckService : IProductionQualityCheckService
{
    private readonly IProductionQualityCheckRepository _qualityCheckRepository;
    private readonly IProductionOrderRepository _orderRepository;

    public ProductionQualityCheckService(IProductionQualityCheckRepository qualityCheckRepository, IProductionOrderRepository orderRepository)
    {
        _qualityCheckRepository = qualityCheckRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<ProductionQualityCheckResponse>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        var items = await _qualityCheckRepository.GetByProductionOrderIdAsync(productionOrderId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ProductionQualityCheckResponse> CreateAsync(CreateProductionQualityCheckRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var orderExists = await _orderRepository.ExistsAsync(x => x.Id == request.ProductionOrderId && !x.IsDeleted, cancellationToken);
        if (!orderExists) throw new InvalidOperationException(ProductionErrorMessages.ProductionOrderDoesNotExist);

        var entity = new ProductionQualityCheck
        {
            ProductionOrderId = request.ProductionOrderId,
            ProductionExecutionId = request.ProductionExecutionId,
            CheckType = request.CheckType,
            Status = request.Status,
            CheckDate = request.CheckDate,
            CheckedByUserId = request.CheckedByUserId,
            ReferenceStandard = request.ReferenceStandard?.Trim(),
            Findings = request.Findings?.Trim(),
            CorrectiveAction = request.CorrectiveAction?.Trim(),
            RequiresRework = request.RequiresRework,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _qualityCheckRepository.AddAsync(entity, cancellationToken);
        await _qualityCheckRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }
}
