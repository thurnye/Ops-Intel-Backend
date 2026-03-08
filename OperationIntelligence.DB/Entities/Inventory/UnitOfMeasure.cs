namespace OperationIntelligence.DB;

public class UnitOfMeasure : AuditableEntity
{
    public string Name { get; set; } = default!;     // e.g. Piece, Box, Kg, Liter
    public string Symbol { get; set; } = default!;   // e.g. pcs, bx, kg, l

    public ICollection<Product> Products { get; set; } = new List<Product>();

}
