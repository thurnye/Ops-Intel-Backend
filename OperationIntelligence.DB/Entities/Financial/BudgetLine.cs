namespace OperationIntelligence.DB;

public class BudgetLine : AuditableEntity
{
    public Guid BudgetId { get; set; }
    public Budget Budget { get; set; } = default!;

    public Guid AccountId { get; set; }
    public ChartOfAccount Account { get; set; } = default!;

    public int PeriodNumber { get; set; }
    public decimal BudgetAmount { get; set; }
}
