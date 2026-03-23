namespace OperationIntelligence.DB;

public class Budget : AuditableEntity
{
    public string BudgetCode { get; set; } = default!;

    public Guid FiscalYearId { get; set; }
    public FiscalYear FiscalYear { get; set; } = default!;

    public Guid DepartmentId { get; set; }
    public Guid? CostCenterId { get; set; }
    public CostCenter? CostCenter { get; set; }

    public string Name { get; set; } = default!;
    public decimal TotalBudgetAmount { get; set; }
    public bool IsApproved { get; set; } = false;

    public ICollection<BudgetLine> Lines { get; set; } = new List<BudgetLine>();
}
