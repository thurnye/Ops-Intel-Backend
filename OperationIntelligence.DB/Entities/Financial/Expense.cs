namespace OperationIntelligence.DB;

public class Expense : AuditableEntity
{
    public string ExpenseNumber { get; set; } = default!;
    public DateTime ExpenseDate { get; set; }

    public Guid ExpenseAccountId { get; set; }
    public ChartOfAccount ExpenseAccount { get; set; } = default!;

    public Guid? CostCenterId { get; set; }
    public CostCenter? CostCenter { get; set; }

    public Guid? VendorId { get; set; }
    public string Category { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }

    public Guid? RelatedJournalEntryId { get; set; }
    public JournalEntry? RelatedJournalEntry { get; set; }
}
