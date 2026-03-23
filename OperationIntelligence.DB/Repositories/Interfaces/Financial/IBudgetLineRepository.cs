namespace OperationIntelligence.DB;

public interface IBudgetLineRepository : IBaseRepository<BudgetLine>
{
    Task<IReadOnlyList<BudgetLine>> GetByBudgetIdAsync(Guid budgetId, CancellationToken cancellationToken = default);
}
