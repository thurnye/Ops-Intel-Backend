using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class UpdateScheduleResourceAssignmentRequestValidator : AbstractValidator<UpdateScheduleResourceAssignmentRequest>
{
    public UpdateScheduleResourceAssignmentRequestValidator()
    {
        RuleFor(x => x.AssignmentRole)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.AssignedEndUtc)
            .GreaterThanOrEqualTo(x => x.AssignedStartUtc);

        RuleFor(x => x.PlannedHours)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ActualHours)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Status)
            .InclusiveBetween(1, 4);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
