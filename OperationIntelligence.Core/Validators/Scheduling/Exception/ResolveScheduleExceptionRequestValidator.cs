using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Exception;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Exception;

public class ResolveScheduleExceptionRequestValidator : AbstractValidator<ResolveScheduleExceptionRequest>
{
    public ResolveScheduleExceptionRequestValidator()
    {
        RuleFor(x => x.ResolvedAtUtc)
            .NotEmpty();

        RuleFor(x => x.ResolutionNotes)
            .MaximumLength(SchedulingValidationConstants.DescriptionMaxLength);
    }
}
