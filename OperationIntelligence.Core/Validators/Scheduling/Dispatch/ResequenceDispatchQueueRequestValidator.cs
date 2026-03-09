using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Dispatch;

public class ResequenceDispatchQueueRequestValidator : AbstractValidator<ResequenceDispatchQueueRequest>
{
    public ResequenceDispatchQueueRequestValidator()
    {
        RuleFor(x => x.WorkCenterId).NotEmpty();

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);

        RuleFor(x => x.Items)
            .NotEmpty();

        RuleForEach(x => x.Items)
            .SetValidator(new DispatchQueueReorderItemRequestValidator());

        RuleFor(x => x.Items)
            .Must(items => items.Select(i => i.QueuePosition).Distinct().Count() == items.Count)
            .WithMessage("Queue positions must be unique.");
    }
}
