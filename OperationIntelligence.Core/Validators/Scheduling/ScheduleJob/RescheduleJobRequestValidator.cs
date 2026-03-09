using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleJob;

public class RescheduleJobRequestValidator : AbstractValidator<RescheduleJobRequest>
{
    public RescheduleJobRequestValidator()
    {
        RuleFor(x => x.NewLatestFinishUtc)
            .GreaterThanOrEqualTo(x => x.NewEarliestStartUtc!.Value)
            .When(x => x.NewEarliestStartUtc.HasValue && x.NewLatestFinishUtc.HasValue);

        RuleFor(x => x.NewPriority!.Value)
            .InclusiveBetween(1, 5)
            .When(x => x.NewPriority.HasValue);

        RuleFor(x => x.ReasonCode)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.ReasonDescription)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
