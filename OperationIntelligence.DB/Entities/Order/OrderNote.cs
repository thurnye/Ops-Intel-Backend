namespace OperationIntelligence.DB;

public class OrderNote : OrderBaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public string Note { get; set; } = default!;
    public bool IsInternal { get; set; } = true;
}