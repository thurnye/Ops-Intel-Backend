namespace OperationIntelligence.DB;

public interface IProductionExecutionRepository : IBaseRepository<ProductionExecution>
{
    Task<ProductionExecution?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionExecution>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionExecution>> GetByStatusAsync(ExecutionStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionExecution>> GetByWorkCenterIdAsync(Guid workCenterId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionExecution>> GetActiveExecutionsAsync(CancellationToken cancellationToken = default);
}
