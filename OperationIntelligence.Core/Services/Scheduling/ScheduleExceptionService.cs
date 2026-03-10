
using OperationIntelligence.Core.Models.Scheduling.Requests.Exception;
using OperationIntelligence.Core.Models.Scheduling.Responses.Exception;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ScheduleExceptionService : IScheduleExceptionService
{
    private readonly IScheduleExceptionRepository _scheduleExceptionRepository;

    public ScheduleExceptionService(IScheduleExceptionRepository scheduleExceptionRepository)
    {
        _scheduleExceptionRepository = scheduleExceptionRepository;
    }

    public async Task<ScheduleExceptionResponse> CreateAsync(CreateScheduleExceptionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ScheduleException
        {
            SchedulePlanId = request.SchedulePlanId,
            ScheduleJobId = request.ScheduleJobId,
            ScheduleOperationId = request.ScheduleOperationId,
            ExceptionType = (ScheduleExceptionType)request.ExceptionType,
            Severity = (ScheduleExceptionSeverity)request.Severity,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            DetectedAtUtc = request.DetectedAtUtc,
            AssignedTo = request.AssignedTo?.Trim(),
            Status = ScheduleExceptionStatus.Open,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _scheduleExceptionRepository.AddAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleExceptionResponse> ResolveAsync(Guid id, ResolveScheduleExceptionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleExceptionRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleExceptionNotFound);

        entity.ResolvedAtUtc = request.ResolvedAtUtc;
        entity.ResolutionNotes = request.ResolutionNotes?.Trim();
        entity.Status = ScheduleExceptionStatus.Resolved;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleExceptionRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleExceptionResponse> UpdateStatusAsync(Guid id, UpdateScheduleExceptionStatusRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleExceptionRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleExceptionNotFound);

        entity.Status = (ScheduleExceptionStatus)request.Status;
        entity.AssignedTo = request.AssignedTo?.Trim();
        entity.ResolutionNotes = request.ResolutionNotes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleExceptionRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleExceptionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleExceptionRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<PagedResponse<ScheduleExceptionResponse>> GetAllAsync(GetScheduleExceptionsRequest request, CancellationToken cancellationToken = default)
    {
        var (items, totalRecords) = await _scheduleExceptionRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.StartDateUtc,
            request.EndDateUtc,
            request.SchedulePlanId,
            request.ScheduleJobId,
            request.ScheduleOperationId,
            request.ExceptionType,
            request.Severity,
            request.Status,
            request.AssignedTo,
            cancellationToken);

        return new PagedResponse<ScheduleExceptionResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            Items = items.Select(MapToResponse).ToList()
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleExceptionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _scheduleExceptionRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private static ScheduleExceptionResponse MapToResponse(ScheduleException entity)
    {
        return new ScheduleExceptionResponse
        {
            Id = entity.Id,
            SchedulePlanId = entity.SchedulePlanId,
            ScheduleJobId = entity.ScheduleJobId,
            ScheduleOperationId = entity.ScheduleOperationId,
            ExceptionType = (int)entity.ExceptionType,
            ExceptionTypeName = entity.ExceptionType.ToString(),
            Severity = (int)entity.Severity,
            SeverityName = entity.Severity.ToString(),
            Title = entity.Title,
            Description = entity.Description,
            DetectedAtUtc = entity.DetectedAtUtc,
            ResolvedAtUtc = entity.ResolvedAtUtc,
            AssignedTo = entity.AssignedTo,
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            ResolutionNotes = entity.ResolutionNotes
        };
    }
}
