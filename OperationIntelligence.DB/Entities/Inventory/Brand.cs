namespace OperationIntelligence.DB;

public class Brand: AuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
