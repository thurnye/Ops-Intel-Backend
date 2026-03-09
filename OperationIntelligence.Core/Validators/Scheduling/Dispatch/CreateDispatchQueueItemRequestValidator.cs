using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Dispatch;

public class CreateDispatchQueueItemRequestValidator : AbstractValidator<CreateDispatchQueueItemRequest>
{
    public CreateDispatchQueueItemRequestValidator()
    {
        RuleFor(x => x.ScheduleOperationId).NotEmpty();
        RuleFor(x => x.WorkCenterId).NotEmpty();

        RuleFor(x => x.QueuePosition)
            .GreaterThan(0);

        RuleFor(x => x.PriorityScore)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DispatchNotes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
