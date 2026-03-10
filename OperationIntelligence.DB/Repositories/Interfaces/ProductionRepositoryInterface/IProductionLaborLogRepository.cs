namespace OperationIntelligence.DB;

public interface IProductionLaborLogRepository : IBaseRepository<ProductionLaborLog>
{
    Task<IReadOnlyList<ProductionLaborLog>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionLaborLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionLaborLog>> GetByWorkDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
