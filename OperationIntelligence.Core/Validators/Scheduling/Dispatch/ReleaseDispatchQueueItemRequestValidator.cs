using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Dispatch;

public class ReleaseDispatchQueueItemRequestValidator : AbstractValidator<ReleaseDispatchQueueItemRequest>
{
    public ReleaseDispatchQueueItemRequestValidator()
    {
        RuleFor(x => x.ReleasedAtUtc)
            .NotEmpty();

        RuleFor(x => x.DispatchNotes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
