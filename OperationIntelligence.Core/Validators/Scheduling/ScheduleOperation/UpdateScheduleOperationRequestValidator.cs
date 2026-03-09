using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class UpdateScheduleOperationRequestValidator : AbstractValidator<UpdateScheduleOperationRequest>
{
    public UpdateScheduleOperationRequestValidator()
    {
        RuleFor(x => x.WorkCenterId).NotEmpty();

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
