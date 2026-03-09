using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductionLaborLogService : IProductionLaborLogService
{
    private readonly IProductionLaborLogRepository _laborLogRepository;
    private readonly IProductionExecutionRepository _executionRepository;

    public ProductionLaborLogService(IProductionLaborLogRepository laborLogRepository, IProductionExecutionRepository executionRepository)
    {
        _laborLogRepository = laborLogRepository;
        _executionRepository = executionRepository;
    }

    public async Task<IReadOnlyList<ProductionLaborLogResponse>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        var items = await _laborLogRepository.GetByProductionExecutionIdAsync(productionExecutionId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ProductionLaborLogResponse> CreateAsync(CreateProductionLaborLogRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var executionExists = await _executionRepository.ExistsAsync(x => x.Id == request.ProductionExecutionId && !x.IsDeleted, cancellationToken);
        if (!executionExists) throw new InvalidOperationException("Production execution does not exist.");

        var entity = new ProductionLaborLog
        {
            ProductionExecutionId = request.ProductionExecutionId,
            UserId = request.UserId,
            HoursWorked = request.HoursWorked,
            HourlyRate = request.HourlyRate,
            WorkDate = request.WorkDate,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _laborLogRepository.AddAsync(entity, cancellationToken);
        await _laborLogRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }
}
