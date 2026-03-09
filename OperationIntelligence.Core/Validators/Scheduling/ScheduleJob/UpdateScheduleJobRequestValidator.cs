using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleJob;

public class UpdateScheduleJobRequestValidator : AbstractValidator<UpdateScheduleJobRequest>
{
    public UpdateScheduleJobRequestValidator()
    {
        RuleFor(x => x.JobName)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);

        RuleFor(x => x.PlannedQuantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.LatestFinishUtc)
            .GreaterThanOrEqualTo(x => x.EarliestStartUtc!.Value)
            .When(x => x.EarliestStartUtc.HasValue && x.LatestFinishUtc.HasValue);

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 5);
    }
}
