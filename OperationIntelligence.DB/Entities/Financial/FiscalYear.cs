namespace OperationIntelligence.DB;

public class FiscalYear : AuditableEntity
{
    public int YearCode { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsClosed { get; set; } = false;

    public ICollection<FiscalPeriod> Periods { get; set; } = new List<FiscalPeriod>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}
