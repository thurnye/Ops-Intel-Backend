using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Exception;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Exception;

public class UpdateScheduleExceptionStatusRequestValidator : AbstractValidator<UpdateScheduleExceptionStatusRequest>
{
    public UpdateScheduleExceptionStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .InclusiveBetween(1, 4);

        RuleFor(x => x.AssignedTo)
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);

        RuleFor(x => x.ResolutionNotes)
            .MaximumLength(SchedulingValidationConstants.DescriptionMaxLength);
    }
}
