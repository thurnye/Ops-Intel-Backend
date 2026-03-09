using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class CreateScheduleOperationConstraintRequestValidator : AbstractValidator<CreateScheduleOperationConstraintRequest>
{
    public CreateScheduleOperationConstraintRequestValidator()
    {
        RuleFor(x => x.ScheduleOperationId).NotEmpty();

        RuleFor(x => x.ConstraintType)
            .InclusiveBetween(1, 6);

        RuleFor(x => x.ReferenceNo)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.DescriptionMaxLength);
    }
}
