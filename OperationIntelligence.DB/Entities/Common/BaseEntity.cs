namespace OperationIntelligence.DB;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
