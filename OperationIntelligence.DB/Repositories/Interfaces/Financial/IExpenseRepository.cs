namespace OperationIntelligence.DB;

public interface IExpenseRepository : IBaseRepository<Expense>
{
    Task<Expense?> GetByExpenseNumberAsync(string expenseNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Expense>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Expense>> GetByCostCenterAsync(Guid costCenterId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByCategoryAsync(string category, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
