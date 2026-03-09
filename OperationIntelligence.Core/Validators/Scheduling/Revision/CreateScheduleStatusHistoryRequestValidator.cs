using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Revision;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Revision;

public class CreateScheduleStatusHistoryRequestValidator : AbstractValidator<CreateScheduleStatusHistoryRequest>
{
    public CreateScheduleStatusHistoryRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.SchedulePlanId.HasValue || x.ScheduleJobId.HasValue || x.ScheduleOperationId.HasValue)
            .WithMessage("At least one of SchedulePlanId, ScheduleJobId, or ScheduleOperationId must be provided.");

        RuleFor(x => x.EntityType)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.OldStatus)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.NewStatus)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Reason)
            .MaximumLength(500);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
