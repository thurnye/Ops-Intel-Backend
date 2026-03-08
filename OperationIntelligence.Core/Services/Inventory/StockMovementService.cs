using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class StockMovementService : IStockMovementService
{
    private readonly IStockMovementRepository _stockMovementRepository;

    public StockMovementService(IStockMovementRepository stockMovementRepository)
    {
        _stockMovementRepository = stockMovementRepository;
    }

    public async Task<IReadOnlyList<StockMovementResponse>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var movements = await _stockMovementRepository.GetByProductIdAsync(productId, cancellationToken);
        return movements.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<StockMovementResponse>> GetByWarehouseIdAsync(
        Guid warehouseId,
        CancellationToken cancellationToken = default)
    {
        var movements = await _stockMovementRepository.GetByWarehouseIdAsync(warehouseId, cancellationToken);
        return movements.Select(MapToResponse).ToList();
    }

    private static StockMovementResponse MapToResponse(StockMovement movement)
    {
        return new StockMovementResponse
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            ProductName = movement.Product?.Name ?? string.Empty,
            WarehouseId = movement.WarehouseId,
            WarehouseName = movement.Warehouse?.Name ?? string.Empty,
            MovementType = movement.MovementType,
            Quantity = movement.Quantity,
            QuantityBefore = movement.QuantityBefore,
            QuantityAfter = movement.QuantityAfter,
            ReferenceNumber = movement.ReferenceNumber,
            Reason = movement.Reason,
            Notes = movement.Notes,
            MovementDateUtc = movement.MovementDateUtc
        };
    }
}
