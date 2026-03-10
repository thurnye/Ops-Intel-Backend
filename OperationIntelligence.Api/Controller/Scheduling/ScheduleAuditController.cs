using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.Audit;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/audit")]
public class ScheduleAuditController : BaseApiController
{
    private readonly IScheduleAuditService _scheduleAuditService;

    public ScheduleAuditController(IScheduleAuditService scheduleAuditService)
    {
        _scheduleAuditService = scheduleAuditService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleAuditLogRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleAuditService.CreateAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _scheduleAuditService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleAuditLogNotFound);

        return OkResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _scheduleAuditService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return PagedOkResponse(result);
    }
}
