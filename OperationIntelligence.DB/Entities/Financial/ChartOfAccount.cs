namespace OperationIntelligence.DB;

public class ChartOfAccount : AuditableEntity
{
    public string AccountCode { get; set; } = default!;
    public string AccountName { get; set; } = default!;
    public AccountType AccountType { get; set; }

    public Guid? ParentAccountId { get; set; }
    public ChartOfAccount? ParentAccount { get; set; }

    public bool IsActive { get; set; } = true;
    public bool AllowManualPosting { get; set; } = true;
    public string? Description { get; set; }

    public ICollection<ChartOfAccount> ChildAccounts { get; set; } = new List<ChartOfAccount>();
    public ICollection<JournalLine> JournalLines { get; set; } = new List<JournalLine>();
    public ICollection<GeneralLedgerEntry> GeneralLedgerEntries { get; set; } = new List<GeneralLedgerEntry>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<BudgetLine> BudgetLines { get; set; } = new List<BudgetLine>();
}
