namespace OperationIntelligence.DB;

public interface IOrderRepository : IOrderBaseRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetByOrderNumberWithDetailsAsync(string orderNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        OrderStatus? status = null,
        OrderType? orderType = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? searchTerm = null,
        OrderStatus? status = null,
        OrderType? orderType = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
}