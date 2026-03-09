using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class ResolveScheduleOperationConstraintRequestValidator : AbstractValidator<ResolveScheduleOperationConstraintRequest>
{
    public ResolveScheduleOperationConstraintRequestValidator()
    {
        RuleFor(x => x.SatisfiedAtUtc)
            .NotNull()
            .When(x => x.IsSatisfied);
    }
}
