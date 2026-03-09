using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.SchedulePlan;

public class CloneSchedulePlanRequestValidator : AbstractValidator<CloneSchedulePlanRequest>
{
    public CloneSchedulePlanRequestValidator()
    {
        RuleFor(x => x.NewPlanNumber)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.CodeMaxLength);

        RuleFor(x => x.NewName)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);

        RuleFor(x => x.Reason)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
