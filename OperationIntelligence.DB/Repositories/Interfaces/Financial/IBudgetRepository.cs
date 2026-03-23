namespace OperationIntelligence.DB;

public interface IBudgetRepository : IBaseRepository<Budget>
{
    Task<Budget?> GetByBudgetCodeAsync(string budgetCode, CancellationToken cancellationToken = default);
    Task<Budget?> GetWithLinesAsync(Guid budgetId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Budget>> GetByFiscalYearAsync(Guid fiscalYearId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Budget>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
}
