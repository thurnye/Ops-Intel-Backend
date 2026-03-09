using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductionMaterialConsumptionService : IProductionMaterialConsumptionService
{
    private readonly IProductionMaterialConsumptionRepository _consumptionRepository;
    private readonly IProductionMaterialIssueRepository _issueRepository;

    public ProductionMaterialConsumptionService(
        IProductionMaterialConsumptionRepository consumptionRepository,
        IProductionMaterialIssueRepository issueRepository)
    {
        _consumptionRepository = consumptionRepository;
        _issueRepository = issueRepository;
    }

    public async Task<IReadOnlyList<ProductionMaterialConsumptionResponse>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        var items = await _consumptionRepository.GetByProductionExecutionIdAsync(productionExecutionId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ProductionMaterialConsumptionResponse> CreateAsync(CreateProductionMaterialConsumptionRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var issueExists = await _issueRepository.ExistsAsync(x => x.Id == request.ProductionMaterialIssueId && !x.IsDeleted, cancellationToken);
        if (!issueExists) throw new InvalidOperationException("Production material issue does not exist.");

        var entity = new ProductionMaterialConsumption
        {
            ProductionMaterialIssueId = request.ProductionMaterialIssueId,
            ProductionExecutionId = request.ProductionExecutionId,
            ConsumedQuantity = request.ConsumedQuantity,
            ConsumptionDate = request.ConsumptionDate,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _consumptionRepository.AddAsync(entity, cancellationToken);
        await _consumptionRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }
}
