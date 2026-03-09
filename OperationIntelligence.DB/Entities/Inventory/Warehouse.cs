namespace OperationIntelligence.DB;

public class Warehouse: AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }

    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? StateOrProvince { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    public ICollection<DeliveryRun> DeliveryRuns { get; set; } = new List<DeliveryRun>();
    public ICollection<DockAppointment> DockAppointments { get; set; } = new List<DockAppointment>();
}
