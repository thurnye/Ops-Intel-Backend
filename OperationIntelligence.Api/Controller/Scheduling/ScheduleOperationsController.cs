using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/operations")]
public class ScheduleOperationsController : BaseApiController
{
    private readonly IScheduleOperationService _scheduleOperationService;

    public ScheduleOperationsController(IScheduleOperationService scheduleOperationService)
    {
        _scheduleOperationService = scheduleOperationService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleOperationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.CreateAsync(request, cancellationToken);
            return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScheduleOperationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.UpdateAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id, [FromBody] StartScheduleOperationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.StartAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("{id:guid}/pause")]
    public async Task<IActionResult> Pause(Guid id, [FromBody] PauseScheduleOperationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.PauseAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteScheduleOperationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.CompleteAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("{id:guid}/reschedule")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleOperationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.RescheduleAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPost("dependencies")]
    public async Task<IActionResult> AddDependency([FromBody] CreateScheduleOperationDependencyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.AddDependencyAsync(request, cancellationToken);
            return CreatedResponse(result);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpDelete("dependencies/{id:guid}")]
    public async Task<IActionResult> RemoveDependency(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _scheduleOperationService.RemoveDependencyAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.OperationDependencyNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.OperationDependencyDeletedSuccessfully });
    }

    [HttpPost("constraints")]
    public async Task<IActionResult> AddConstraint([FromBody] CreateScheduleOperationConstraintRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleOperationService.AddConstraintAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPost("constraints/{id:guid}/resolve")]
    public async Task<IActionResult> ResolveConstraint(Guid id, [FromBody] ResolveScheduleOperationConstraintRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.ResolveConstraintAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpDelete("constraints/{id:guid}")]
    public async Task<IActionResult> RemoveConstraint(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _scheduleOperationService.RemoveConstraintAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.OperationConstraintNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.OperationConstraintDeletedSuccessfully });
    }

    [HttpPost("resource-options")]
    public async Task<IActionResult> AddResourceOption([FromBody] CreateScheduleOperationResourceOptionRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleOperationService.AddResourceOptionAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpDelete("resource-options/{id:guid}")]
    public async Task<IActionResult> RemoveResourceOption(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _scheduleOperationService.RemoveResourceOptionAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.OperationResourceOptionNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.OperationResourceOptionDeletedSuccessfully });
    }

    [HttpPost("resource-assignments")]
    public async Task<IActionResult> AddResourceAssignment([FromBody] CreateScheduleResourceAssignmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.AddResourceAssignmentAsync(request, cancellationToken);
            return CreatedResponse(result);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPut("resource-assignments/{id:guid}")]
    public async Task<IActionResult> UpdateResourceAssignment(Guid id, [FromBody] UpdateScheduleResourceAssignmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleOperationService.UpdateResourceAssignmentAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpDelete("resource-assignments/{id:guid}")]
    public async Task<IActionResult> RemoveResourceAssignment(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _scheduleOperationService.RemoveResourceAssignmentAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ResourceAssignmentNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.ResourceAssignmentDeletedSuccessfully });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _scheduleOperationService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleOperationNotFound);

        return OkResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetScheduleOperationsRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleOperationService.GetAllAsync(request, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _scheduleOperationService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleOperationNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.ScheduleOperationDeletedSuccessfully });
    }
}
