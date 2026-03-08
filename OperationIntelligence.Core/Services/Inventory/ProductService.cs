using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var isSkuUnique = await _productRepository.IsSkuUniqueAsync(request.SKU, null, cancellationToken);
        if (!isSkuUnique)
            throw new InvalidOperationException(InventoryErrorMessages.ProductSkuAlreadyExists(request.SKU));

        var isBarcodeUnique = await _productRepository.IsBarcodeUniqueAsync(request.Barcode, null, cancellationToken);
        if (!isBarcodeUnique)
            throw new InvalidOperationException(InventoryErrorMessages.ProductBarcodeAlreadyExists(request.Barcode));

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            SKU = request.SKU,
            Barcode = request.Barcode,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            UnitOfMeasureId = request.UnitOfMeasureId,
            CostPrice = request.CostPrice,
            SellingPrice = request.SellingPrice,
            TaxRate = request.TaxRate,
            ReorderLevel = request.ReorderLevel,
            ReorderQuantity = request.ReorderQuantity,
            TrackInventory = request.TrackInventory,
            AllowBackOrder = request.AllowBackOrder,
            IsSerialized = request.IsSerialized,
            IsBatchTracked = request.IsBatchTracked,
            IsPerishable = request.IsPerishable,
            Weight = request.Weight,
            Length = request.Length,
            Width = request.Width,
            Height = request.Height,
            Status = request.Status,
            ThumbnailImageUrl = request.ThumbnailImageUrl
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var createdProduct = await _productRepository.GetProductWithDetailsAsync(product.Id, cancellationToken);
        if (createdProduct == null)
            throw new InvalidOperationException(InventoryErrorMessages.ProductCreatedButNotRetrieved);

        return MapToProductResponse(createdProduct);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(productId, cancellationToken);
        return product == null ? null : MapToProductResponse(product);
    }

    public async Task<PagedResponse<ProductListItemResponse>> GetPagedAsync(
        ProductQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var products = await _productRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            request.SearchTerm,
            request.CategoryId,
            request.Status,
            cancellationToken);

        var totalRecords = await _productRepository.CountAsync(
            request.SearchTerm,
            request.CategoryId,
            request.Status,
            cancellationToken);

        return new PagedResponse<ProductListItemResponse>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            Items = products.Select(MapToProductListItemResponse).ToList()
        };
    }

    public async Task<ProductResponse?> UpdateAsync(
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingProduct == null)
            return null;

        var isSkuUnique = await _productRepository.IsSkuUniqueAsync(request.SKU, request.Id, cancellationToken);
        if (!isSkuUnique)
            throw new InvalidOperationException(InventoryErrorMessages.ProductSkuAlreadyExists(request.SKU));

        var isBarcodeUnique = await _productRepository.IsBarcodeUniqueAsync(request.Barcode, request.Id, cancellationToken);
        if (!isBarcodeUnique)
            throw new InvalidOperationException(InventoryErrorMessages.ProductBarcodeAlreadyExists(request.Barcode));

        existingProduct.Name = request.Name;
        existingProduct.Description = request.Description;
        existingProduct.SKU = request.SKU;
        existingProduct.Barcode = request.Barcode;
        existingProduct.CategoryId = request.CategoryId;
        existingProduct.BrandId = request.BrandId;
        existingProduct.UnitOfMeasureId = request.UnitOfMeasureId;
        existingProduct.CostPrice = request.CostPrice;
        existingProduct.SellingPrice = request.SellingPrice;
        existingProduct.TaxRate = request.TaxRate;
        existingProduct.ReorderLevel = request.ReorderLevel;
        existingProduct.ReorderQuantity = request.ReorderQuantity;
        existingProduct.TrackInventory = request.TrackInventory;
        existingProduct.AllowBackOrder = request.AllowBackOrder;
        existingProduct.IsSerialized = request.IsSerialized;
        existingProduct.IsBatchTracked = request.IsBatchTracked;
        existingProduct.IsPerishable = request.IsPerishable;
        existingProduct.Weight = request.Weight;
        existingProduct.Length = request.Length;
        existingProduct.Width = request.Width;
        existingProduct.Height = request.Height;
        existingProduct.Status = request.Status;
        existingProduct.ThumbnailImageUrl = request.ThumbnailImageUrl;
        existingProduct.UpdatedAtUtc = DateTime.UtcNow;

        _productRepository.Update(existingProduct);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var updatedProduct = await _productRepository.GetProductWithDetailsAsync(existingProduct.Id, cancellationToken);
        return updatedProduct == null ? null : MapToProductResponse(updatedProduct);
    }

    private static ProductListItemResponse MapToProductListItemResponse(Product product)
    {
        return new ProductListItemResponse
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Barcode = product.Barcode,
            CategoryName = product.Category?.Name ?? string.Empty,
            BrandName = product.Brand?.Name,
            SellingPrice = product.SellingPrice,
            Status = product.Status,
            ThumbnailImageUrl = product.ThumbnailImageUrl
        };
    }

    private static ProductResponse MapToProductResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Barcode = product.Barcode,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            BrandId = product.BrandId,
            BrandName = product.Brand?.Name,
            UnitOfMeasureId = product.UnitOfMeasureId,
            UnitOfMeasureName = product.UnitOfMeasure?.Name ?? string.Empty,
            CostPrice = product.CostPrice,
            SellingPrice = product.SellingPrice,
            TaxRate = product.TaxRate,
            ReorderLevel = product.ReorderLevel,
            ReorderQuantity = product.ReorderQuantity,
            TrackInventory = product.TrackInventory,
            AllowBackOrder = product.AllowBackOrder,
            IsSerialized = product.IsSerialized,
            IsBatchTracked = product.IsBatchTracked,
            IsPerishable = product.IsPerishable,
            Weight = product.Weight,
            Length = product.Length,
            Width = product.Width,
            Height = product.Height,
            Status = product.Status,
            ThumbnailImageUrl = product.ThumbnailImageUrl,
            Images = product.Images
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new ProductImageResponse
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    FileName = i.FileName,
                    FileUrl = i.FileUrl,
                    ContentType = i.ContentType,
                    FileSizeInBytes = i.FileSizeInBytes,
                    IsPrimary = i.IsPrimary,
                    DisplayOrder = i.DisplayOrder,
                    AltText = i.AltText
                })
                .ToList(),
            InventoryStocks = product.InventoryStocks
                .Select(s => new InventoryStockResponse
                {
                    Id = s.Id,
                    ProductId = s.ProductId,
                    WarehouseId = s.WarehouseId,
                    WarehouseName = s.Warehouse?.Name ?? string.Empty,
                    QuantityOnHand = s.QuantityOnHand,
                    QuantityReserved = s.QuantityReserved,
                    QuantityAvailable = s.QuantityAvailable,
                    QuantityDamaged = s.QuantityDamaged
                })
                .ToList()
        };
    }
}
