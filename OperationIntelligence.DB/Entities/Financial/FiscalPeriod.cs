namespace OperationIntelligence.DB;

public class FiscalPeriod : AuditableEntity
{
    public Guid FiscalYearId { get; set; }
    public FiscalYear FiscalYear { get; set; } = default!;

    public int PeriodNumber { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public FiscalPeriodStatus Status { get; set; } = FiscalPeriodStatus.Open;
    public DateTime? ClosedAtUtc { get; set; }

    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    public ICollection<GeneralLedgerEntry> GeneralLedgerEntries { get; set; } = new List<GeneralLedgerEntry>();
}
