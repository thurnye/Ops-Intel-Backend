namespace OperationIntelligence.DB;

public class GeneralLedgerEntry : AuditableEntity
{
    public Guid JournalEntryId { get; set; }
    public JournalEntry JournalEntry { get; set; } = default!;

    public Guid JournalLineId { get; set; }
    public JournalLine JournalLine { get; set; } = default!;

    public Guid AccountId { get; set; }
    public ChartOfAccount Account { get; set; } = default!;

    public Guid FiscalPeriodId { get; set; }
    public FiscalPeriod FiscalPeriod { get; set; } = default!;

    public DateTime PostingDate { get; set; }

    public Guid? CostCenterId { get; set; }
    public CostCenter? CostCenter { get; set; }

    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }

    public string CurrencyCode { get; set; } = "CAD";
    public decimal ExchangeRate { get; set; } = 1m;
}
