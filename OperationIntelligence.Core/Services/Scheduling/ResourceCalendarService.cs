using Microsoft.Extensions.Logging;
using OperationIntelligence.Core.Models.Scheduling.Requests.ResourceCalendar;
using OperationIntelligence.Core.Models.Scheduling.Responses.ResourceCalendar;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ResourceCalendarService : IResourceCalendarService
{
    private readonly IResourceCalendarRepository _resourceCalendarRepository;
    private readonly IResourceCalendarExceptionRepository _resourceCalendarExceptionRepository;
    private readonly ILogger<ResourceCalendarService> _logger;

    public ResourceCalendarService(
        IResourceCalendarRepository resourceCalendarRepository,
        IResourceCalendarExceptionRepository resourceCalendarExceptionRepository,
        ILogger<ResourceCalendarService> logger)
    {
        _resourceCalendarRepository = resourceCalendarRepository;
        _resourceCalendarExceptionRepository = resourceCalendarExceptionRepository;
        _logger = logger;
    }

    public async Task<ResourceCalendarResponse> CreateAsync(CreateResourceCalendarRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ResourceCalendar
        {
            ResourceId = request.ResourceId,
            ResourceType = (ResourceType)request.ResourceType,
            CalendarName = request.CalendarName.Trim(),
            TimeZone = request.TimeZone.Trim(),
            MondayEnabled = request.MondayEnabled,
            TuesdayEnabled = request.TuesdayEnabled,
            WednesdayEnabled = request.WednesdayEnabled,
            ThursdayEnabled = request.ThursdayEnabled,
            FridayEnabled = request.FridayEnabled,
            SaturdayEnabled = request.SaturdayEnabled,
            SundayEnabled = request.SundayEnabled,
            DefaultStartTime = request.DefaultStartTime,
            DefaultEndTime = request.DefaultEndTime,
            IsDefault = request.IsDefault,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _resourceCalendarRepository.AddAsync(entity, cancellationToken);
        var created = await _resourceCalendarRepository.GetByIdWithDetailsAsync(entity.Id, cancellationToken) ?? entity;
        return MapToResponse(created);
    }

    public async Task<ResourceCalendarResponse> UpdateAsync(Guid id, UpdateResourceCalendarRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _resourceCalendarRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ResourceCalendarNotFound);

        entity.CalendarName = request.CalendarName.Trim();
        entity.TimeZone = request.TimeZone.Trim();
        entity.MondayEnabled = request.MondayEnabled;
        entity.TuesdayEnabled = request.TuesdayEnabled;
        entity.WednesdayEnabled = request.WednesdayEnabled;
        entity.ThursdayEnabled = request.ThursdayEnabled;
        entity.FridayEnabled = request.FridayEnabled;
        entity.SaturdayEnabled = request.SaturdayEnabled;
        entity.SundayEnabled = request.SundayEnabled;
        entity.DefaultStartTime = request.DefaultStartTime;
        entity.DefaultEndTime = request.DefaultEndTime;
        entity.IsDefault = request.IsDefault;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _resourceCalendarRepository.UpdateAsync(entity, cancellationToken);
        var updated = await _resourceCalendarRepository.GetByIdWithDetailsAsync(entity.Id, cancellationToken) ?? entity;
        return MapToResponse(updated);
    }

    public async Task<ResourceCalendarExceptionResponse> AddExceptionAsync(CreateResourceCalendarExceptionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ResourceCalendarException
        {
            ResourceCalendarId = request.ResourceCalendarId,
            ExceptionStartUtc = request.ExceptionStartUtc,
            ExceptionEndUtc = request.ExceptionEndUtc,
            ExceptionType = (CalendarExceptionType)request.ExceptionType,
            IsWorkingException = request.IsWorkingException,
            Reason = request.Reason.Trim(),
            Notes = request.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _resourceCalendarExceptionRepository.AddAsync(entity, cancellationToken);

        return new ResourceCalendarExceptionResponse
        {
            Id = entity.Id,
            ResourceCalendarId = entity.ResourceCalendarId,
            ExceptionStartUtc = entity.ExceptionStartUtc,
            ExceptionEndUtc = entity.ExceptionEndUtc,
            ExceptionType = (int)entity.ExceptionType,
            ExceptionTypeName = entity.ExceptionType.ToString(),
            IsWorkingException = entity.IsWorkingException,
            Reason = entity.Reason,
            Notes = entity.Notes
        };
    }

    public async Task<ResourceCalendarResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _resourceCalendarRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _resourceCalendarRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _resourceCalendarRepository.DeleteAsync(entity, cancellationToken);
        _logger.LogInformation("Deleted resource calendar {ResourceCalendarId}", id);
        return true;
    }

    public async Task<bool> DeleteExceptionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _resourceCalendarExceptionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _resourceCalendarExceptionRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private static ResourceCalendarResponse MapToResponse(ResourceCalendar entity)
    {
        return new ResourceCalendarResponse
        {
            Id = entity.Id,
            ResourceId = entity.ResourceId,
            ResourceType = (int)entity.ResourceType,
            ResourceTypeName = entity.ResourceType.ToString(),
            CalendarName = entity.CalendarName,
            TimeZone = entity.TimeZone,
            MondayEnabled = entity.MondayEnabled,
            TuesdayEnabled = entity.TuesdayEnabled,
            WednesdayEnabled = entity.WednesdayEnabled,
            ThursdayEnabled = entity.ThursdayEnabled,
            FridayEnabled = entity.FridayEnabled,
            SaturdayEnabled = entity.SaturdayEnabled,
            SundayEnabled = entity.SundayEnabled,
            DefaultStartTime = entity.DefaultStartTime,
            DefaultEndTime = entity.DefaultEndTime,
            IsDefault = entity.IsDefault,
            Exceptions = entity.Exceptions?.Select(x => new ResourceCalendarExceptionResponse
            {
                Id = x.Id,
                ResourceCalendarId = x.ResourceCalendarId,
                ExceptionStartUtc = x.ExceptionStartUtc,
                ExceptionEndUtc = x.ExceptionEndUtc,
                ExceptionType = (int)x.ExceptionType,
                ExceptionTypeName = x.ExceptionType.ToString(),
                IsWorkingException = x.IsWorkingException,
                Reason = x.Reason,
                Notes = x.Notes
            }).ToList() ?? new List<ResourceCalendarExceptionResponse>()
        };
    }
}
