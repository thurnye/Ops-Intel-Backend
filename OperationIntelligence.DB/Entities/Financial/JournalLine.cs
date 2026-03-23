namespace OperationIntelligence.DB;

public class JournalLine : AuditableEntity
{
    public Guid JournalEntryId { get; set; }
    public JournalEntry JournalEntry { get; set; } = default!;

    public int LineNumber { get; set; }

    public Guid AccountId { get; set; }
    public ChartOfAccount Account { get; set; } = default!;

    public Guid? CostCenterId { get; set; }
    public CostCenter? CostCenter { get; set; }

    public string Description { get; set; } = default!;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }

    public string CurrencyCode { get; set; } = "CAD";
    public decimal ExchangeRate { get; set; } = 1m;

    public ICollection<GeneralLedgerEntry> GeneralLedgerEntries { get; set; } = new List<GeneralLedgerEntry>();
}
