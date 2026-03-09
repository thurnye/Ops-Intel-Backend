using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class RescheduleOperationRequestValidator : AbstractValidator<RescheduleOperationRequest>
{
    public RescheduleOperationRequestValidator()
    {
        RuleFor(x => x.NewPlannedStartUtc)
            .NotEmpty();

        RuleFor(x => x.NewPlannedEndUtc)
            .GreaterThanOrEqualTo(x => x.NewPlannedStartUtc);

        RuleFor(x => x.NewPriorityScore!.Value)
            .GreaterThanOrEqualTo(0)
            .When(x => x.NewPriorityScore.HasValue);

        RuleFor(x => x.ReasonCode)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.ReasonDescription)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
