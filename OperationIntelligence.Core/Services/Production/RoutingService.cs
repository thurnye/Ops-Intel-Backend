using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class RoutingService : IRoutingService
{
    private readonly IRoutingRepository _routingRepository;
    private readonly IRoutingStepRepository _routingStepRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWorkCenterRepository _workCenterRepository;

    public RoutingService(
        IRoutingRepository routingRepository,
        IRoutingStepRepository routingStepRepository,
        IProductRepository productRepository,
        IWorkCenterRepository workCenterRepository)
    {
        _routingRepository = routingRepository;
        _routingStepRepository = routingStepRepository;
        _productRepository = productRepository;
        _workCenterRepository = workCenterRepository;
    }

    public async Task<RoutingResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _routingRepository.GetWithStepsAsync(id, cancellationToken);
        return entity is null || entity.IsDeleted ? null : entity.ToResponse();
    }

    public async Task<PagedResponse<RoutingSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var query = _routingRepository.Query().AsNoTracking().Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedAtUtc);
        var total = await query.CountAsync(cancellationToken);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => new RoutingSummaryResponse
        {
            Id = x.Id,
            RoutingCode = x.RoutingCode,
            Name = x.Name,
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            ProductSku = x.Product.SKU,
            Version = x.Version,
            IsActive = x.IsActive,
            IsDefault = x.IsDefault,
            EffectiveFrom = x.EffectiveFrom,
            EffectiveTo = x.EffectiveTo
        }).ToListAsync(cancellationToken);

        return new PagedResponse<RoutingSummaryResponse> { PageNumber = pageNumber, PageSize = pageSize, TotalRecords = total, Items = items };
    }

    public async Task<RoutingMetricsSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var items = await _routingRepository.Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => new { x.IsActive, x.IsDefault, x.ProductId })
            .ToListAsync(cancellationToken);

        return new RoutingMetricsSummaryResponse
        {
            TotalRoutings = items.Count,
            ActiveRoutings = items.Count(x => x.IsActive),
            DefaultRoutings = items.Count(x => x.IsDefault),
            ProductCoverage = items.Select(x => x.ProductId).Distinct().Count()
        };
    }

    public async Task<RoutingResponse> CreateAsync(CreateRoutingRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var productExists = await _productRepository.ExistsAsync(x => x.Id == request.ProductId && !x.IsDeleted, cancellationToken);
        if (!productExists) throw new InvalidOperationException(ProductionErrorMessages.ProductDoesNotExist);

        var codeExists = await _routingRepository.RoutingCodeExistsAsync(request.RoutingCode.Trim(), null, cancellationToken);
        if (codeExists) throw new InvalidOperationException(ProductionErrorMessages.RoutingCodeAlreadyExists);

        var entity = new Routing
        {
            RoutingCode = request.RoutingCode.Trim(),
            Name = request.Name.Trim(),
            ProductId = request.ProductId,
            Version = request.Version,
            IsActive = request.IsActive,
            IsDefault = request.IsDefault,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _routingRepository.AddAsync(entity, cancellationToken);
        await _routingRepository.SaveChangesAsync(cancellationToken);

        var created = await _routingRepository.GetWithStepsAsync(entity.Id, cancellationToken) ?? entity;
        return created.ToResponse();
    }

    public async Task<RoutingStepResponse> AddStepAsync(CreateRoutingStepRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var routing = await _routingRepository.GetByIdAsync(request.RoutingId, cancellationToken);
        if (routing is null || routing.IsDeleted) throw new InvalidOperationException(ProductionErrorMessages.RoutingDoesNotExist);

        var workCenterExists = await _workCenterRepository.ExistsAsync(x => x.Id == request.WorkCenterId && !x.IsDeleted && x.IsActive, cancellationToken);
        if (!workCenterExists) throw new InvalidOperationException(ProductionErrorMessages.WorkCenterDoesNotExistOrIsInactive);

        var sequenceExists = await _routingStepRepository.GetByRoutingAndSequenceAsync(request.RoutingId, request.Sequence, cancellationToken);
        if (sequenceExists is not null) throw new InvalidOperationException(ProductionErrorMessages.SequenceAlreadyExistsInRouting);

        var entity = new RoutingStep
        {
            RoutingId = request.RoutingId,
            Sequence = request.Sequence,
            OperationCode = request.OperationCode.Trim(),
            OperationName = request.OperationName.Trim(),
            WorkCenterId = request.WorkCenterId,
            SetupTimeMinutes = request.SetupTimeMinutes,
            RunTimeMinutesPerUnit = request.RunTimeMinutesPerUnit,
            QueueTimeMinutes = request.QueueTimeMinutes,
            WaitTimeMinutes = request.WaitTimeMinutes,
            MoveTimeMinutes = request.MoveTimeMinutes,
            RequiredOperators = request.RequiredOperators,
            IsOutsourced = request.IsOutsourced,
            IsQualityCheckpointRequired = request.IsQualityCheckpointRequired,
            Instructions = request.Instructions?.Trim(),
            Notes = request.Notes?.Trim(),
            CreatedBy = createdBy
        };

        await _routingStepRepository.AddAsync(entity, cancellationToken);
        await _routingStepRepository.SaveChangesAsync(cancellationToken);

        return entity.ToResponse();
    }

    public async Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _routingRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;

        _routingRepository.Update(entity);
        await _routingRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
