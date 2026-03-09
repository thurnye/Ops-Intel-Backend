using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class CreateScheduleOperationDependencyRequestValidator : AbstractValidator<CreateScheduleOperationDependencyRequest>
{
    public CreateScheduleOperationDependencyRequestValidator()
    {
        RuleFor(x => x.PredecessorOperationId).NotEmpty();
        RuleFor(x => x.SuccessorOperationId).NotEmpty();

        RuleFor(x => x)
            .Must(x => x.PredecessorOperationId != x.SuccessorOperationId)
            .WithMessage("Predecessor and successor operations must be different.");

        RuleFor(x => x.DependencyType)
            .InclusiveBetween(1, 4);

        RuleFor(x => x.LagMinutes)
            .GreaterThanOrEqualTo(0);
    }
}
