using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductionOutputService : IProductionOutputService
{
    private readonly IProductionOutputRepository _outputRepository;
    private readonly IProductionOrderRepository _orderRepository;

    public ProductionOutputService(IProductionOutputRepository outputRepository, IProductionOrderRepository orderRepository)
    {
        _outputRepository = outputRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<ProductionOutputResponse>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        var items = await _outputRepository.GetByProductionOrderIdAsync(productionOrderId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ProductionOutputResponse> CreateAsync(CreateProductionOutputRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var orderExists = await _orderRepository.ExistsAsync(x => x.Id == request.ProductionOrderId && !x.IsDeleted, cancellationToken);
        if (!orderExists) throw new InvalidOperationException(ProductionErrorMessages.ProductionOrderDoesNotExist);

        var entity = new ProductionOutput
        {
            ProductionOrderId = request.ProductionOrderId,
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            QuantityProduced = request.QuantityProduced,
            UnitOfMeasureId = request.UnitOfMeasureId,
            BatchNumber = request.BatchNumber?.Trim(),
            LotNumber = request.LotNumber?.Trim(),
            OutputDate = request.OutputDate,
            IsFinalOutput = request.IsFinalOutput,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _outputRepository.AddAsync(entity, cancellationToken);
        await _outputRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }
}
