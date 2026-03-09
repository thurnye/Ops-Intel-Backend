namespace OperationIntelligence.DB;

public interface IProductionScrapRepository : IBaseRepository<ProductionScrap>
{
    Task<IReadOnlyList<ProductionScrap>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionScrap>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionScrap>> GetByReasonAsync(ScrapReasonType reason, CancellationToken cancellationToken = default);
}
