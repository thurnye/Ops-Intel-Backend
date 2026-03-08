using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductImageService : IProductImageService
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductRepository _productRepository;

    public ProductImageService(
        IProductImageRepository productImageRepository,
        IProductRepository productRepository)
    {
        _productImageRepository = productImageRepository;
        _productRepository = productRepository;
    }

    public async Task<ProductImageResponse> AddAsync(AddProductImageRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException(InventoryErrorMessages.ProductNotFound);

        if (request.IsPrimary)
        {
            var existingImages = await _productImageRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
            foreach (var image in existingImages.Where(i => i.IsPrimary))
            {
                image.IsPrimary = false;
                image.UpdatedAtUtc = DateTime.UtcNow;
                _productImageRepository.Update(image);
            }
        }

        var newImage = new ProductImage
        {
            ProductId = request.ProductId,
            FileName = request.FileName,
            FileUrl = request.FileUrl,
            ContentType = request.ContentType,
            FileSizeInBytes = request.FileSizeInBytes,
            IsPrimary = request.IsPrimary,
            DisplayOrder = request.DisplayOrder,
            AltText = request.AltText
        };

        await _productImageRepository.AddAsync(newImage, cancellationToken);
        await _productImageRepository.SaveChangesAsync(cancellationToken);

        return Map(newImage);
    }

    public async Task<IReadOnlyList<ProductImageResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var images = await _productImageRepository.GetByProductIdAsync(productId, cancellationToken);
        return images.Select(Map).ToList();
    }

    public async Task<bool> DeleteAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetByIdAsync(imageId, cancellationToken);
        if (image == null)
            return false;

        image.IsDeleted = true;
        image.DeletedAtUtc = DateTime.UtcNow;
        _productImageRepository.Update(image);
        await _productImageRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SetPrimaryAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetByIdAsync(imageId, cancellationToken);
        if (image == null)
            return false;

        var images = await _productImageRepository.GetByProductIdAsync(image.ProductId, cancellationToken);
        foreach (var item in images)
        {
            item.IsPrimary = item.Id == imageId;
            item.UpdatedAtUtc = DateTime.UtcNow;
            _productImageRepository.Update(item);
        }

        await _productImageRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProductImageResponse Map(ProductImage image) => new()
    {
        Id = image.Id,
        ProductId = image.ProductId,
        FileName = image.FileName,
        FileUrl = image.FileUrl,
        ContentType = image.ContentType,
        FileSizeInBytes = image.FileSizeInBytes,
        IsPrimary = image.IsPrimary,
        DisplayOrder = image.DisplayOrder,
        AltText = image.AltText
    };
}
