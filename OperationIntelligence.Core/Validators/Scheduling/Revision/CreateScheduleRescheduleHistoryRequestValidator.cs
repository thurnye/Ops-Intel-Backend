using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Revision;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Revision;

public class CreateScheduleRescheduleHistoryRequestValidator : AbstractValidator<CreateScheduleRescheduleHistoryRequest>
{
    public CreateScheduleRescheduleHistoryRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.SchedulePlanId.HasValue || x.ScheduleJobId.HasValue || x.ScheduleOperationId.HasValue)
            .WithMessage("At least one of SchedulePlanId, ScheduleJobId, or ScheduleOperationId must be provided.");

        RuleFor(x => x.OldPlannedEndUtc)
            .GreaterThanOrEqualTo(x => x.OldPlannedStartUtc!.Value)
            .When(x => x.OldPlannedStartUtc.HasValue && x.OldPlannedEndUtc.HasValue);

        RuleFor(x => x.NewPlannedEndUtc)
            .GreaterThanOrEqualTo(x => x.NewPlannedStartUtc!.Value)
            .When(x => x.NewPlannedStartUtc.HasValue && x.NewPlannedEndUtc.HasValue);

        RuleFor(x => x.ReasonCode)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.ReasonDescription)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
