namespace OperationIntelligence.Core;

public class OrderNoteResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Note { get; set; } = default!;
    public bool IsInternal { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}