namespace OperationIntelligence.Core;

public class CreateOrderNoteRequest
{
    public Guid OrderId { get; set; }
    public string Note { get; set; } = default!;
    public bool IsInternal { get; set; } = true;
    public string CreatedBy { get; set; } = default!;
}