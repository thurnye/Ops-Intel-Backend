namespace OperationIntelligence.DB;

public class JournalEntry : AuditableEntity
{
    public string JournalNumber { get; set; } = default!;
    public DateTime EntryDate { get; set; }
    public DateTime PostingDate { get; set; }

    public Guid FiscalPeriodId { get; set; }
    public FiscalPeriod FiscalPeriod { get; set; } = default!;

    public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;

    public FinanceSourceModule SourceModule { get; set; } = FinanceSourceModule.Manual;
    public string? SourceReferenceType { get; set; }
    public Guid? SourceReferenceId { get; set; }

    public string Memo { get; set; } = default!;

    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }

    public string? PostedByUserId { get; set; }
    public DateTime? PostedAtUtc { get; set; }

    public bool IsReversal { get; set; } = false;
    public Guid? ReversedJournalEntryId { get; set; }
    public JournalEntry? ReversedJournalEntry { get; set; }

    public ICollection<JournalEntry> ReversalEntries { get; set; } = new List<JournalEntry>();
    public ICollection<JournalLine> Lines { get; set; } = new List<JournalLine>();
    public ICollection<GeneralLedgerEntry> GeneralLedgerEntries { get; set; } = new List<GeneralLedgerEntry>();
}
