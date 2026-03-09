using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductionScrapService : IProductionScrapService
{
    private readonly IProductionScrapRepository _scrapRepository;
    private readonly IProductionOrderRepository _orderRepository;

    public ProductionScrapService(IProductionScrapRepository scrapRepository, IProductionOrderRepository orderRepository)
    {
        _scrapRepository = scrapRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<ProductionScrapResponse>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        var items = await _scrapRepository.GetByProductionOrderIdAsync(productionOrderId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ProductionScrapResponse> CreateAsync(CreateProductionScrapRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var orderExists = await _orderRepository.ExistsAsync(x => x.Id == request.ProductionOrderId && !x.IsDeleted, cancellationToken);
        if (!orderExists) throw new InvalidOperationException("Production order does not exist.");

        var entity = new ProductionScrap
        {
            ProductionOrderId = request.ProductionOrderId,
            ProductionExecutionId = request.ProductionExecutionId,
            ProductId = request.ProductId,
            ScrapQuantity = request.ScrapQuantity,
            UnitOfMeasureId = request.UnitOfMeasureId,
            Reason = request.Reason,
            ReasonDescription = request.ReasonDescription?.Trim(),
            ScrapDate = request.ScrapDate,
            IsReworkable = request.IsReworkable,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _scrapRepository.AddAsync(entity, cancellationToken);
        await _scrapRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }
}
