using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductSupplierService : IProductSupplierService
{
    private readonly IProductSupplierRepository _productSupplierRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public ProductSupplierService(
        IProductSupplierRepository productSupplierRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository)
    {
        _productSupplierRepository = productSupplierRepository;
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<ProductSupplierResponse> AssignAsync(AssignProductSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException(InventoryErrorMessages.ProductNotFound);

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null)
            throw new InvalidOperationException(InventoryErrorMessages.SupplierNotFound);

        var existing = await _productSupplierRepository.GetByProductAndSupplierAsync(request.ProductId, request.SupplierId, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException(InventoryErrorMessages.SupplierAlreadyAssignedToProduct);

        if (request.IsPreferredSupplier)
        {
            var currentAssignments = await _productSupplierRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
            foreach (var assignment in currentAssignments.Where(x => x.IsPreferredSupplier))
            {
                assignment.IsPreferredSupplier = false;
                assignment.UpdatedAtUtc = DateTime.UtcNow;
                _productSupplierRepository.Update(assignment);
            }
        }

        var entity = new ProductSupplier
        {
            ProductId = request.ProductId,
            SupplierId = request.SupplierId,
            SupplierProductCode = request.SupplierProductCode,
            SupplierPrice = request.SupplierPrice,
            LeadTimeInDays = request.LeadTimeInDays,
            IsPreferredSupplier = request.IsPreferredSupplier
        };

        await _productSupplierRepository.AddAsync(entity, cancellationToken);
        await _productSupplierRepository.SaveChangesAsync(cancellationToken);

        entity.Product = product;
        entity.Supplier = supplier;

        return Map(entity);
    }

    public async Task<ProductSupplierResponse?> UpdateAsync(UpdateProductSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _productSupplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null)
            return null;

        if (request.IsPreferredSupplier)
        {
            var currentAssignments = await _productSupplierRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
            foreach (var assignment in currentAssignments.Where(x => x.IsPreferredSupplier && x.Id != request.Id))
            {
                assignment.IsPreferredSupplier = false;
                assignment.UpdatedAtUtc = DateTime.UtcNow;
                _productSupplierRepository.Update(assignment);
            }
        }

        entity.ProductId = request.ProductId;
        entity.SupplierId = request.SupplierId;
        entity.SupplierProductCode = request.SupplierProductCode;
        entity.SupplierPrice = request.SupplierPrice;
        entity.LeadTimeInDays = request.LeadTimeInDays;
        entity.IsPreferredSupplier = request.IsPreferredSupplier;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        _productSupplierRepository.Update(entity);
        await _productSupplierRepository.SaveChangesAsync(cancellationToken);

        var product = await _productRepository.GetByIdAsync(entity.ProductId, cancellationToken);
        var supplier = await _supplierRepository.GetByIdAsync(entity.SupplierId, cancellationToken);
        if (product != null)
            entity.Product = product;
        if (supplier != null)
            entity.Supplier = supplier;

        return Map(entity);
    }

    public async Task<bool> RemoveAsync(Guid productSupplierId, CancellationToken cancellationToken = default)
    {
        var entity = await _productSupplierRepository.GetByIdAsync(productSupplierId, cancellationToken);
        if (entity == null)
            return false;

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        _productSupplierRepository.Update(entity);
        await _productSupplierRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<ProductSupplierResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var assignments = await _productSupplierRepository.GetByProductIdAsync(productId, cancellationToken);
        return assignments.Select(Map).ToList();
    }

    private static ProductSupplierResponse Map(ProductSupplier entity) => new()
    {
        Id = entity.Id,
        ProductId = entity.ProductId,
        ProductName = entity.Product?.Name ?? string.Empty,
        SupplierId = entity.SupplierId,
        SupplierName = entity.Supplier?.Name ?? string.Empty,
        SupplierProductCode = entity.SupplierProductCode,
        SupplierPrice = entity.SupplierPrice,
        LeadTimeInDays = entity.LeadTimeInDays,
        IsPreferredSupplier = entity.IsPreferredSupplier
    };
}
