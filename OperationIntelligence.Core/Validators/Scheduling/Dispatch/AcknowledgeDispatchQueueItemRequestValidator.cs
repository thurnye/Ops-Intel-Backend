using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Dispatch;

public class AcknowledgeDispatchQueueItemRequestValidator : AbstractValidator<AcknowledgeDispatchQueueItemRequest>
{
    public AcknowledgeDispatchQueueItemRequestValidator()
    {
        RuleFor(x => x.AcknowledgedAtUtc)
            .NotEmpty();

        RuleFor(x => x.DispatchNotes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
