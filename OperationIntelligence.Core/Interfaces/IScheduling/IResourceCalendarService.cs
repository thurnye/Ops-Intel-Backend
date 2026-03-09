using OperationIntelligence.Core.Models.Scheduling.Requests.ResourceCalendar;
using OperationIntelligence.Core.Models.Scheduling.Responses.ResourceCalendar;

namespace OperationIntelligence.Core;

public interface IResourceCalendarService
{
    Task<ResourceCalendarResponse> CreateAsync(
        CreateResourceCalendarRequest request,
        CancellationToken cancellationToken = default);

    Task<ResourceCalendarResponse> UpdateAsync(
        Guid id,
        UpdateResourceCalendarRequest request,
        CancellationToken cancellationToken = default);

    Task<ResourceCalendarExceptionResponse> AddExceptionAsync(
        CreateResourceCalendarExceptionRequest request,
        CancellationToken cancellationToken = default);

    Task<ResourceCalendarResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteExceptionAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
