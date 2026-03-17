using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;
using OperationIntelligence.Core.Models.Scheduling.Responses.Material;
using OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleJob;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ScheduleJobService : IScheduleJobService
{
    private readonly IScheduleJobRepository _scheduleJobRepository;
    private readonly OperationIntelligenceDbContext _dbContext;
    private readonly ILogger<ScheduleJobService> _logger;

    public ScheduleJobService(
        IScheduleJobRepository scheduleJobRepository,
        OperationIntelligenceDbContext dbContext,
        ILogger<ScheduleJobService> logger)
    {
        _scheduleJobRepository = scheduleJobRepository;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ScheduleJobResponse> CreateAsync(CreateScheduleJobRequest request, CancellationToken cancellationToken = default)
    {
        if (await _scheduleJobRepository.ExistsByJobNumberAsync(request.JobNumber.Trim(), cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.ScheduleJobNumberAlreadyExists);

        var entity = new ScheduleJob
        {
            SchedulePlanId = request.SchedulePlanId,
            ProductionOrderId = request.ProductionOrderId,
            OrderId = request.OrderId,
            OrderItemId = request.OrderItemId,
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            JobNumber = request.JobNumber.Trim(),
            JobName = request.JobName.Trim(),
            Notes = request.Notes?.Trim(),
            PlannedQuantity = request.PlannedQuantity,
            EarliestStartUtc = request.EarliestStartUtc,
            LatestFinishUtc = request.LatestFinishUtc,
            DueDateUtc = request.DueDateUtc,
            Priority = (SchedulePriority)request.Priority,
            Status = ScheduleJobStatus.Unscheduled,
            IsRushOrder = request.IsRushOrder,
            QualityHold = request.QualityHold,
            MaterialsReady = false,
            MaterialReadinessStatus = MaterialReadinessStatus.NotChecked,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _scheduleJobRepository.AddAsync(entity, cancellationToken);
        _logger.LogInformation("Created schedule job {ScheduleJobId}", entity.Id);

        var created = await _scheduleJobRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;
        return MapToResponse(created);
    }

    public Task<BulkCreateResponse<ScheduleJobResponse>> CreateBulkAsync(
        BulkCreateRequest<CreateScheduleJobRequest> request,
        CancellationToken cancellationToken = default) =>
        BulkCreateExecutor.ExecuteAsync(
            request.Items,
            (item, token) => CreateAsync(item, token),
            cancellationToken);

    public async Task<ScheduleJobResponse> UpdateAsync(Guid id, UpdateScheduleJobRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleJobRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleJobNotFound);

        entity.JobName = request.JobName.Trim();
        entity.Notes = request.Notes?.Trim();
        entity.PlannedQuantity = request.PlannedQuantity;
        entity.EarliestStartUtc = request.EarliestStartUtc;
        entity.LatestFinishUtc = request.LatestFinishUtc;
        entity.DueDateUtc = request.DueDateUtc;
        entity.Priority = (SchedulePriority)request.Priority;
        entity.IsRushOrder = request.IsRushOrder;
        entity.QualityHold = request.QualityHold;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleJobRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleJobResponse> ReleaseAsync(Guid id, ReleaseScheduleJobRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleJobRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleJobNotFound);

        if (entity.QualityHold)
            throw new InvalidOperationException(SchedulingErrorMessages.ScheduleJobCannotBeReleasedWhileOnQualityHold);

        entity.Status = ScheduleJobStatus.Released;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _scheduleJobRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleJobResponse> PauseAsync(Guid id, PauseScheduleJobRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleJobRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleJobNotFound);

        entity.Status = ScheduleJobStatus.Paused;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _scheduleJobRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleJobResponse> CancelAsync(Guid id, CancelScheduleJobRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleJobRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleJobNotFound);

        entity.Status = ScheduleJobStatus.Cancelled;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _scheduleJobRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleJobResponse> RescheduleAsync(Guid id, RescheduleJobRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleJobRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleJobNotFound);

        entity.EarliestStartUtc = request.NewEarliestStartUtc;
        entity.LatestFinishUtc = request.NewLatestFinishUtc;
        entity.DueDateUtc = request.NewDueDateUtc;
        if (request.NewPriority.HasValue)
            entity.Priority = (SchedulePriority)request.NewPriority.Value;

        entity.Status = ScheduleJobStatus.Scheduled;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleJobRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleJobDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleJobRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null) return null;

        return new ScheduleJobDetailResponse
        {
            Id = entity.Id,
            SchedulePlanId = entity.SchedulePlanId,
            ProductionOrderId = entity.ProductionOrderId,
            ProductionOrderNumber = entity.ProductionOrder?.ProductionOrderNumber ?? string.Empty,
            OrderId = entity.OrderId,
            OrderNumber = entity.Order?.OrderNumber,
            OrderItemId = entity.OrderItemId,
            ProductId = entity.ProductId,
            ProductName = entity.Product?.Name ?? string.Empty,
            WarehouseId = entity.WarehouseId,
            WarehouseName = entity.Warehouse?.Name ?? string.Empty,
            JobNumber = entity.JobNumber,
            JobName = entity.JobName,
            Notes = entity.Notes,
            PlannedQuantity = entity.PlannedQuantity,
            CompletedQuantity = entity.CompletedQuantity,
            ScrappedQuantity = entity.ScrappedQuantity,
            EarliestStartUtc = entity.EarliestStartUtc,
            LatestFinishUtc = entity.LatestFinishUtc,
            DueDateUtc = entity.DueDateUtc,
            Priority = (int)entity.Priority,
            PriorityName = entity.Priority.ToString(),
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            MaterialsReady = entity.MaterialsReady,
            MaterialReadinessStatus = (int)entity.MaterialReadinessStatus,
            MaterialReadinessStatusName = entity.MaterialReadinessStatus.ToString(),
            MaterialsCheckedAtUtc = entity.MaterialsCheckedAtUtc,
            QualityHold = entity.QualityHold,
            IsRushOrder = entity.IsRushOrder,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc ?? entity.CreatedAtUtc,
            TotalOperations = entity.ScheduleOperations?.Count ?? 0,
            TotalExceptions = entity.ScheduleExceptions?.Count ?? 0,
            TotalMaterialChecks = entity.MaterialChecks?.Count ?? 0,
            Operations = entity.ScheduleOperations?.Select(x => new ScheduleOperationBriefResponse
            {
                Id = x.Id,
                SequenceNo = x.SequenceNo,
                OperationCode = x.OperationCode,
                OperationName = x.OperationName,
                PlannedStartUtc = x.PlannedStartUtc,
                PlannedEndUtc = x.PlannedEndUtc,
                Status = (int)x.Status,
                StatusName = x.Status.ToString()
            }).ToList() ?? new List<ScheduleOperationBriefResponse>(),
            MaterialChecks = entity.MaterialChecks?.Select(x => new ScheduleMaterialCheckResponse
            {
                Id = x.Id,
                ScheduleJobId = x.ScheduleJobId,
                ScheduleOperationId = x.ScheduleOperationId,
                ProductionOrderId = x.ProductionOrderId,
                RoutingStepId = x.RoutingStepId,
                MaterialProductId = x.MaterialProductId,
                MaterialProductName = x.MaterialProduct?.Name ?? string.Empty,
                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse?.Name ?? string.Empty,
                RequiredQuantity = x.RequiredQuantity,
                AvailableQuantity = x.AvailableQuantity,
                ReservedQuantity = x.ReservedQuantity,
                ShortageQuantity = x.ShortageQuantity,
                Status = (int)x.Status,
                StatusName = x.Status.ToString(),
                ExpectedAvailabilityDateUtc = x.ExpectedAvailabilityDateUtc,
                Notes = x.Notes,
                CheckedAtUtc = x.CheckedAtUtc
            }).ToList() ?? new List<ScheduleMaterialCheckResponse>()
        };
    }

    public async Task<PagedResponse<ScheduleJobResponse>> GetAllAsync(GetScheduleJobsRequest request, CancellationToken cancellationToken = default)
    {
        var (items, totalRecords) = await _scheduleJobRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.StartDateUtc,
            request.EndDateUtc,
            request.SchedulePlanId,
            request.ProductionOrderId,
            request.OrderId,
            request.ProductId,
            request.WarehouseId,
            request.Status,
            request.Priority,
            request.MaterialsReady,
            request.MaterialReadinessStatus,
            request.QualityHold,
            request.IsRushOrder,
            cancellationToken);

        return new PagedResponse<ScheduleJobResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            Items = items.Select(MapToResponse).ToList()
        };
    }

    public async Task<DispatchMetricsSummaryResponse> GetDispatchSummaryAsync(CancellationToken cancellationToken = default)
    {
        var jobStatuses = await _dbContext.ScheduleJobs
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);

        var exceptionStatuses = await _dbContext.ScheduleExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);

        return new DispatchMetricsSummaryResponse
        {
            TotalJobs = jobStatuses.Count,
            ReleasedJobs = jobStatuses.Count(x => x == ScheduleJobStatus.Released),
            RunningJobs = jobStatuses.Count(x => x == ScheduleJobStatus.Running),
            OpenBlockers = exceptionStatuses.Count(x =>
                x == ScheduleExceptionStatus.Open ||
                x == ScheduleExceptionStatus.Investigating)
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleJobRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _scheduleJobRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private static ScheduleJobResponse MapToResponse(ScheduleJob entity)
    {
        return new ScheduleJobResponse
        {
            Id = entity.Id,
            SchedulePlanId = entity.SchedulePlanId,
            ProductionOrderId = entity.ProductionOrderId,
            ProductionOrderNumber = entity.ProductionOrder?.ProductionOrderNumber ?? string.Empty,
            OrderId = entity.OrderId,
            OrderNumber = entity.Order?.OrderNumber,
            OrderItemId = entity.OrderItemId,
            ProductId = entity.ProductId,
            ProductName = entity.Product?.Name ?? string.Empty,
            WarehouseId = entity.WarehouseId,
            WarehouseName = entity.Warehouse?.Name ?? string.Empty,
            JobNumber = entity.JobNumber,
            JobName = entity.JobName,
            Notes = entity.Notes,
            PlannedQuantity = entity.PlannedQuantity,
            CompletedQuantity = entity.CompletedQuantity,
            ScrappedQuantity = entity.ScrappedQuantity,
            EarliestStartUtc = entity.EarliestStartUtc,
            LatestFinishUtc = entity.LatestFinishUtc,
            DueDateUtc = entity.DueDateUtc,
            Priority = (int)entity.Priority,
            PriorityName = entity.Priority.ToString(),
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            MaterialsReady = entity.MaterialsReady,
            MaterialReadinessStatus = (int)entity.MaterialReadinessStatus,
            MaterialReadinessStatusName = entity.MaterialReadinessStatus.ToString(),
            MaterialsCheckedAtUtc = entity.MaterialsCheckedAtUtc,
            QualityHold = entity.QualityHold,
            IsRushOrder = entity.IsRushOrder,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc ?? entity.CreatedAtUtc
        };
    }
}
