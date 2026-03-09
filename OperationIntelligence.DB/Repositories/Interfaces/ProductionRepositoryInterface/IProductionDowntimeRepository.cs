namespace OperationIntelligence.DB;

public interface IProductionDowntimeRepository : IBaseRepository<ProductionDowntime>
{
    Task<IReadOnlyList<ProductionDowntime>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionDowntime>> GetByReasonAsync(DowntimeReasonType reason, CancellationToken cancellationToken = default);
}
