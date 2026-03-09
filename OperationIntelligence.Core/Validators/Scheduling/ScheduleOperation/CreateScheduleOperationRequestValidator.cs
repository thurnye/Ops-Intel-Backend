using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class CreateScheduleOperationRequestValidator : AbstractValidator<CreateScheduleOperationRequest>
{
    public CreateScheduleOperationRequestValidator()
    {
        RuleFor(x => x.ScheduleJobId).NotEmpty();
        RuleFor(x => x.RoutingStepId).NotEmpty();
        RuleFor(x => x.WorkCenterId).NotEmpty();

        RuleFor(x => x.SequenceNo)
            .GreaterThan(0);

        RuleFor(x => x.OperationCode)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.CodeMaxLength);

        RuleFor(x => x.OperationName)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);

        RuleFor(x => x.PlannedEndUtc)
            .GreaterThanOrEqualTo(x => x.PlannedStartUtc);

        RuleFor(x => x.SetupTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RunTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.QueueTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.WaitTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MoveTimeMinutes).GreaterThanOrEqualTo(0);

        RuleFor(x => x.PlannedQuantity).GreaterThanOrEqualTo(0);

        RuleFor(x => x.PriorityScore).GreaterThanOrEqualTo(0);

        RuleFor(x => x.ConstraintReason)
            .MaximumLength(500);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
