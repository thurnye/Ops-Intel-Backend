using FluentValidation;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class InventoryStockService : IInventoryStockService
{
    private readonly IInventoryStockRepository _inventoryStockRepository;
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IValidator<StockInRequest> _stockInValidator;
    private readonly IValidator<StockOutRequest> _stockOutValidator;

    public InventoryStockService(
        IInventoryStockRepository inventoryStockRepository,
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IValidator<StockInRequest> stockInValidator,
        IValidator<StockOutRequest> stockOutValidator)
    {
        _inventoryStockRepository = inventoryStockRepository;
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _stockInValidator = stockInValidator;
        _stockOutValidator = stockOutValidator;
    }

    public async Task<InventoryStockResponse> StockInAsync(
        StockInRequest request,
        CancellationToken cancellationToken = default)
    {
        await _stockInValidator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException(InventoryErrorMessages.ProductNotFound);

        var stock = await _inventoryStockRepository.GetByProductAndWarehouseAsync(
            request.ProductId,
            request.WarehouseId,
            cancellationToken);

        if (stock == null)
        {
            stock = new InventoryStock
            {
                ProductId = request.ProductId,
                WarehouseId = request.WarehouseId,
                QuantityOnHand = 0,
                QuantityReserved = 0,
                QuantityAvailable = 0,
                QuantityDamaged = 0
            };

            await _inventoryStockRepository.AddAsync(stock, cancellationToken);
        }

        var quantityBefore = stock.QuantityOnHand;
        stock.QuantityOnHand += request.Quantity;
        stock.QuantityAvailable = stock.QuantityOnHand - stock.QuantityReserved;
        stock.LastStockUpdatedAtUtc = DateTime.UtcNow;

        _inventoryStockRepository.Update(stock);

        var movement = new StockMovement
        {
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            MovementType = StockMovementType.StockIn,
            Quantity = request.Quantity,
            QuantityBefore = quantityBefore,
            QuantityAfter = stock.QuantityOnHand,
            ReferenceNumber = request.ReferenceNumber,
            Reason = request.Reason,
            Notes = request.Notes,
            MovementDateUtc = DateTime.UtcNow
        };

        await _stockMovementRepository.AddAsync(movement, cancellationToken);
        await _inventoryStockRepository.SaveChangesAsync(cancellationToken);

        return new InventoryStockResponse
        {
            Id = stock.Id,
            ProductId = stock.ProductId,
            WarehouseId = stock.WarehouseId,
            WarehouseName = stock.Warehouse?.Name ?? string.Empty,
            QuantityOnHand = stock.QuantityOnHand,
            QuantityReserved = stock.QuantityReserved,
            QuantityAvailable = stock.QuantityAvailable,
            QuantityDamaged = stock.QuantityDamaged
        };
    }

    public async Task<InventoryStockResponse> StockOutAsync(
        StockOutRequest request,
        CancellationToken cancellationToken = default)
    {
        await _stockOutValidator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException(InventoryErrorMessages.ProductNotFound);

        var stock = await _inventoryStockRepository.GetByProductAndWarehouseAsync(
            request.ProductId,
            request.WarehouseId,
            cancellationToken);

        if (stock == null)
            throw new InvalidOperationException(InventoryErrorMessages.InventoryStockNotFound);

        if (!product.AllowBackOrder && stock.QuantityAvailable < request.Quantity)
            throw new InvalidOperationException(InventoryErrorMessages.InsufficientAvailableStock);

        var quantityBefore = stock.QuantityOnHand;
        stock.QuantityOnHand -= request.Quantity;
        stock.QuantityAvailable = stock.QuantityOnHand - stock.QuantityReserved;
        stock.LastStockUpdatedAtUtc = DateTime.UtcNow;

        _inventoryStockRepository.Update(stock);

        var movement = new StockMovement
        {
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            MovementType = StockMovementType.StockOut,
            Quantity = request.Quantity,
            QuantityBefore = quantityBefore,
            QuantityAfter = stock.QuantityOnHand,
            ReferenceNumber = request.ReferenceNumber,
            Reason = request.Reason,
            Notes = request.Notes,
            MovementDateUtc = DateTime.UtcNow
        };

        await _stockMovementRepository.AddAsync(movement, cancellationToken);
        await _inventoryStockRepository.SaveChangesAsync(cancellationToken);

        return new InventoryStockResponse
        {
            Id = stock.Id,
            ProductId = stock.ProductId,
            WarehouseId = stock.WarehouseId,
            WarehouseName = stock.Warehouse?.Name ?? string.Empty,
            QuantityOnHand = stock.QuantityOnHand,
            QuantityReserved = stock.QuantityReserved,
            QuantityAvailable = stock.QuantityAvailable,
            QuantityDamaged = stock.QuantityDamaged
        };
    }

    public async Task<IReadOnlyList<InventoryStockResponse>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var stocks = await _inventoryStockRepository.GetByProductIdAsync(productId, cancellationToken);
        return stocks.Select(s => new InventoryStockResponse
        {
            Id = s.Id,
            ProductId = s.ProductId,
            WarehouseId = s.WarehouseId,
            WarehouseName = s.Warehouse?.Name ?? string.Empty,
            QuantityOnHand = s.QuantityOnHand,
            QuantityReserved = s.QuantityReserved,
            QuantityAvailable = s.QuantityAvailable,
            QuantityDamaged = s.QuantityDamaged
        }).ToList();
    }
}
