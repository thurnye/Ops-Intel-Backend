using Microsoft.Extensions.Logging;
using OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;
using OperationIntelligence.Core.Models.Scheduling.Responses.SchedulePlan;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class SchedulePlanService : ISchedulePlanService
{
    private readonly ISchedulePlanRepository _schedulePlanRepository;
    private readonly ILogger<SchedulePlanService> _logger;

    public SchedulePlanService(ISchedulePlanRepository schedulePlanRepository, ILogger<SchedulePlanService> logger)
    {
        _schedulePlanRepository = schedulePlanRepository;
        _logger = logger;
    }

    public async Task<SchedulePlanResponse> CreateAsync(CreateSchedulePlanRequest request, CancellationToken cancellationToken = default)
    {
        if (await _schedulePlanRepository.ExistsByPlanNumberAsync(request.PlanNumber.Trim(), cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.SchedulePlanNumberAlreadyExists);

        var entity = new SchedulePlan
        {
            PlanNumber = request.PlanNumber.Trim(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            WarehouseId = request.WarehouseId,
            PlanningStartDateUtc = request.PlanningStartDateUtc,
            PlanningEndDateUtc = request.PlanningEndDateUtc,
            GenerationMode = (ScheduleGenerationMode)request.GenerationMode,
            SchedulingStrategy = (SchedulingStrategy)request.SchedulingStrategy,
            AutoSequenceEnabled = request.AutoSequenceEnabled,
            AutoDispatchEnabled = request.AutoDispatchEnabled,
            TimeZone = request.TimeZone.Trim(),
            Status = SchedulePlanStatus.Draft,
            VersionNumber = 1,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _schedulePlanRepository.AddAsync(entity, cancellationToken);
        _logger.LogInformation("Created schedule plan {SchedulePlanId}", entity.Id);

        var created = await _schedulePlanRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;
        return MapToResponse(created);
    }

    public Task<BulkCreateResponse<SchedulePlanResponse>> CreateBulkAsync(
        BulkCreateRequest<CreateSchedulePlanRequest> request,
        CancellationToken cancellationToken = default) =>
        BulkCreateExecutor.ExecuteAsync(
            request.Items,
            (item, token) => CreateAsync(item, token),
            cancellationToken);

    public async Task<SchedulePlanResponse> UpdateAsync(Guid id, UpdateSchedulePlanRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _schedulePlanRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.SchedulePlanNotFound);

        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();
        entity.PlanningStartDateUtc = request.PlanningStartDateUtc;
        entity.PlanningEndDateUtc = request.PlanningEndDateUtc;
        entity.GenerationMode = (ScheduleGenerationMode)request.GenerationMode;
        entity.SchedulingStrategy = (SchedulingStrategy)request.SchedulingStrategy;
        entity.AutoSequenceEnabled = request.AutoSequenceEnabled;
        entity.AutoDispatchEnabled = request.AutoDispatchEnabled;
        entity.TimeZone = request.TimeZone.Trim();
        entity.IsActive = request.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _schedulePlanRepository.UpdateAsync(entity, cancellationToken);
        _logger.LogInformation("Updated schedule plan {SchedulePlanId}", entity.Id);

        var updated = await _schedulePlanRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;
        return MapToResponse(updated);
    }

    public async Task<SchedulePlanResponse> PublishAsync(Guid id, PublishSchedulePlanRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _schedulePlanRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.SchedulePlanNotFound);

        entity.Status = SchedulePlanStatus.Published;
        entity.ApprovedAtUtc = DateTime.UtcNow;
        entity.ApprovedBy = request.ApprovedBy?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _schedulePlanRepository.UpdateAsync(entity, cancellationToken);
        _logger.LogInformation("Published schedule plan {SchedulePlanId}", entity.Id);

        var updated = await _schedulePlanRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;
        return MapToResponse(updated);
    }

    public async Task<SchedulePlanResponse> CloneAsync(Guid id, CloneSchedulePlanRequest request, CancellationToken cancellationToken = default)
    {
        var source = await _schedulePlanRepository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.SourceSchedulePlanNotFound);

        if (await _schedulePlanRepository.ExistsByPlanNumberAsync(request.NewPlanNumber.Trim(), cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.NewSchedulePlanNumberAlreadyExists);

        var clone = new SchedulePlan
        {
            PlanNumber = request.NewPlanNumber.Trim(),
            Name = request.NewName.Trim(),
            Description = source.Description,
            WarehouseId = source.WarehouseId,
            PlanningStartDateUtc = source.PlanningStartDateUtc,
            PlanningEndDateUtc = source.PlanningEndDateUtc,
            GenerationMode = source.GenerationMode,
            SchedulingStrategy = source.SchedulingStrategy,
            AutoSequenceEnabled = source.AutoSequenceEnabled,
            AutoDispatchEnabled = source.AutoDispatchEnabled,
            TimeZone = source.TimeZone,
            Status = SchedulePlanStatus.Draft,
            VersionNumber = source.VersionNumber + 1,
            ParentPlanId = source.Id,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _schedulePlanRepository.AddAsync(clone, cancellationToken);
        _logger.LogInformation("Cloned schedule plan {SourceId} to {CloneId}", source.Id, clone.Id);

        var created = await _schedulePlanRepository.GetByIdAsync(clone.Id, cancellationToken) ?? clone;
        return MapToResponse(created);
    }

    public async Task<SchedulePlanDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _schedulePlanRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null) return null;

        return new SchedulePlanDetailResponse
        {
            Id = entity.Id,
            PlanNumber = entity.PlanNumber,
            Name = entity.Name,
            Description = entity.Description,
            WarehouseId = entity.WarehouseId,
            WarehouseName = entity.Warehouse?.Name ?? string.Empty,
            PlanningStartDateUtc = entity.PlanningStartDateUtc,
            PlanningEndDateUtc = entity.PlanningEndDateUtc,
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            GenerationMode = (int)entity.GenerationMode,
            GenerationModeName = entity.GenerationMode.ToString(),
            SchedulingStrategy = (int)entity.SchedulingStrategy,
            SchedulingStrategyName = entity.SchedulingStrategy.ToString(),
            AutoSequenceEnabled = entity.AutoSequenceEnabled,
            AutoDispatchEnabled = entity.AutoDispatchEnabled,
            VersionNumber = entity.VersionNumber,
            ParentPlanId = entity.ParentPlanId,
            TimeZone = entity.TimeZone,
            ApprovedAtUtc = entity.ApprovedAtUtc,
            ApprovedBy = entity.ApprovedBy,
            IsActive = entity.IsActive,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc ?? entity.CreatedAtUtc,
            TotalJobs = entity.ScheduleJobs?.Count ?? 0,
            TotalOperations = entity.ScheduleJobs?.Sum(x => x.ScheduleOperations.Count) ?? 0,
            TotalExceptions = entity.ScheduleExceptions?.Count ?? 0,
            TotalRevisions = entity.ScheduleRevisions?.Count ?? 0,
            Jobs = entity.ScheduleJobs?.Select(x => new ScheduleJobSummaryResponse
            {
                Id = x.Id,
                JobNumber = x.JobNumber,
                JobName = x.JobName,
                PlannedQuantity = x.PlannedQuantity,
                Status = (int)x.Status,
                StatusName = x.Status.ToString(),
                DueDateUtc = x.DueDateUtc
            }).ToList() ?? new List<ScheduleJobSummaryResponse>()
        };
    }

    public async Task<PagedResponse<SchedulePlanResponse>> GetAllAsync(GetSchedulePlansRequest request, CancellationToken cancellationToken = default)
    {
        var (items, totalRecords) = await _schedulePlanRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.StartDateUtc,
            request.EndDateUtc,
            request.WarehouseId,
            request.Status,
            request.GenerationMode,
            request.SchedulingStrategy,
            request.IsActive,
            cancellationToken);

        return new PagedResponse<SchedulePlanResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            Items = items.Select(MapToResponse).ToList()
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _schedulePlanRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _schedulePlanRepository.DeleteAsync(entity, cancellationToken);
        _logger.LogInformation("Deleted schedule plan {SchedulePlanId}", id);
        return true;
    }

    private static SchedulePlanResponse MapToResponse(SchedulePlan entity)
    {
        return new SchedulePlanResponse
        {
            Id = entity.Id,
            PlanNumber = entity.PlanNumber,
            Name = entity.Name,
            Description = entity.Description,
            WarehouseId = entity.WarehouseId,
            WarehouseName = entity.Warehouse?.Name ?? string.Empty,
            PlanningStartDateUtc = entity.PlanningStartDateUtc,
            PlanningEndDateUtc = entity.PlanningEndDateUtc,
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            GenerationMode = (int)entity.GenerationMode,
            GenerationModeName = entity.GenerationMode.ToString(),
            SchedulingStrategy = (int)entity.SchedulingStrategy,
            SchedulingStrategyName = entity.SchedulingStrategy.ToString(),
            AutoSequenceEnabled = entity.AutoSequenceEnabled,
            AutoDispatchEnabled = entity.AutoDispatchEnabled,
            VersionNumber = entity.VersionNumber,
            ParentPlanId = entity.ParentPlanId,
            TimeZone = entity.TimeZone,
            ApprovedAtUtc = entity.ApprovedAtUtc,
            ApprovedBy = entity.ApprovedBy,
            IsActive = entity.IsActive,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc ?? entity.CreatedAtUtc
        };
    }
}
