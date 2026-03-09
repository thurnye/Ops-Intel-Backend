using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class CreateScheduleResourceAssignmentRequestValidator : AbstractValidator<CreateScheduleResourceAssignmentRequest>
{
    public CreateScheduleResourceAssignmentRequestValidator()
    {
        RuleFor(x => x.ScheduleOperationId).NotEmpty();
        RuleFor(x => x.ResourceId).NotEmpty();

        RuleFor(x => x.ResourceType)
            .InclusiveBetween(1, 6);

        RuleFor(x => x.AssignmentRole)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.AssignedEndUtc)
            .GreaterThanOrEqualTo(x => x.AssignedStartUtc);

        RuleFor(x => x.PlannedHours)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
