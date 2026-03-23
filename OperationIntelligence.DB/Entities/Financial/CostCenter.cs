namespace OperationIntelligence.DB;

public class CostCenter : AuditableEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<JournalLine> JournalLines { get; set; } = new List<JournalLine>();
    public ICollection<GeneralLedgerEntry> GeneralLedgerEntries { get; set; } = new List<GeneralLedgerEntry>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
