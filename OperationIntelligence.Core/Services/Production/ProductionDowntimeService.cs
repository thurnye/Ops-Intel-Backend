using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductionDowntimeService : IProductionDowntimeService
{
    private readonly IProductionDowntimeRepository _downtimeRepository;
    private readonly IProductionExecutionRepository _executionRepository;

    public ProductionDowntimeService(IProductionDowntimeRepository downtimeRepository, IProductionExecutionRepository executionRepository)
    {
        _downtimeRepository = downtimeRepository;
        _executionRepository = executionRepository;
    }

    public async Task<IReadOnlyList<ProductionDowntimeResponse>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        var items = await _downtimeRepository.GetByProductionExecutionIdAsync(productionExecutionId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ProductionDowntimeResponse> CreateAsync(CreateProductionDowntimeRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var executionExists = await _executionRepository.ExistsAsync(x => x.Id == request.ProductionExecutionId && !x.IsDeleted, cancellationToken);
        if (!executionExists) throw new InvalidOperationException("Production execution does not exist.");

        var entity = new ProductionDowntime
        {
            ProductionExecutionId = request.ProductionExecutionId,
            Reason = request.Reason,
            ReasonDescription = request.ReasonDescription?.Trim(),
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            DurationMinutes = (decimal)Math.Max(0, (request.EndTime - request.StartTime).TotalMinutes),
            IsPlanned = request.IsPlanned,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _downtimeRepository.AddAsync(entity, cancellationToken);
        await _downtimeRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }
}
