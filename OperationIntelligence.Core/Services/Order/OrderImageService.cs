using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class OrderImageService : IOrderImageService
{
    private readonly IOrderImageRepository _orderImageRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderImageService(
        IOrderImageRepository orderImageRepository,
        IOrderRepository orderRepository)
    {
        _orderImageRepository = orderImageRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderImageResponse> AddAsync(CreateOrderImageRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        if (request.FileSizeBytes <= 0)
            throw new InvalidOperationException(OrderErrorMessages.InvalidFileSize);

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "application/pdf" };
        if (!allowedTypes.Contains(request.ContentType))
            throw new InvalidOperationException(OrderErrorMessages.UnsupportedFileType);

        if (request.IsPrimary)
        {
            var existing = await _orderImageRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
            foreach (var item in existing.Where(x => x.IsPrimary))
            {
                item.IsPrimary = false;
                item.UpdatedAtUtc = DateTime.UtcNow;
                _orderImageRepository.Update(item);
            }
        }

        var entity = new OrderImage
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            FileName = request.FileName,
            OriginalFileName = request.OriginalFileName,
            FileExtension = request.FileExtension,
            ContentType = request.ContentType,
            FileSizeBytes = request.FileSizeBytes,
            StoragePath = request.StoragePath,
            PublicUrl = request.PublicUrl,
            ImageType = request.ImageType,
            Caption = request.Caption,
            IsPrimary = request.IsPrimary,
            UploadedAtUtc = DateTime.UtcNow,
            UploadedBy = request.UploadedBy,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _orderImageRepository.AddAsync(entity, cancellationToken);
        await _orderImageRepository.SaveChangesAsync(cancellationToken);

        return new OrderImageResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            FileName = entity.FileName,
            PublicUrl = entity.PublicUrl,
            ImageType = entity.ImageType,
            Caption = entity.Caption,
            IsPrimary = entity.IsPrimary
        };
    }

    public async Task<OrderImageResponse> SetPrimaryAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var image = await _orderImageRepository.GetByIdAsync(imageId, cancellationToken);
        if (image == null || !image.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.ImageNotFound);

        var allImages = await _orderImageRepository.GetByOrderIdAsync(image.OrderId, cancellationToken);
        foreach (var item in allImages)
        {
            item.IsPrimary = item.Id == image.Id;
            item.UpdatedAtUtc = DateTime.UtcNow;
            _orderImageRepository.Update(item);
        }

        await _orderImageRepository.SaveChangesAsync(cancellationToken);

        return new OrderImageResponse
        {
            Id = image.Id,
            OrderId = image.OrderId,
            FileName = image.FileName,
            PublicUrl = image.PublicUrl,
            ImageType = image.ImageType,
            Caption = image.Caption,
            IsPrimary = true
        };
    }

    public async Task RemoveAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var image = await _orderImageRepository.GetByIdAsync(imageId, cancellationToken);
        if (image == null || !image.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.ImageNotFound);

        image.IsActive = false;
        image.UpdatedAtUtc = DateTime.UtcNow;

        _orderImageRepository.Update(image);
        await _orderImageRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderImageResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var rows = await _orderImageRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return rows.Select(image => new OrderImageResponse
        {
            Id = image.Id,
            OrderId = image.OrderId,
            FileName = image.FileName,
            PublicUrl = image.PublicUrl,
            ImageType = image.ImageType,
            Caption = image.Caption,
            IsPrimary = image.IsPrimary
        }).ToList();
    }
}