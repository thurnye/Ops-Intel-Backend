using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Exception;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Exception;

public class CreateScheduleExceptionRequestValidator : AbstractValidator<CreateScheduleExceptionRequest>
{
    public CreateScheduleExceptionRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.SchedulePlanId.HasValue || x.ScheduleJobId.HasValue || x.ScheduleOperationId.HasValue)
            .WithMessage("At least one of SchedulePlanId, ScheduleJobId, or ScheduleOperationId must be provided.");

        RuleFor(x => x.ExceptionType)
            .InclusiveBetween(1, 8);

        RuleFor(x => x.Severity)
            .InclusiveBetween(1, 4);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.DescriptionMaxLength);

        RuleFor(x => x.AssignedTo)
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);
    }
}
