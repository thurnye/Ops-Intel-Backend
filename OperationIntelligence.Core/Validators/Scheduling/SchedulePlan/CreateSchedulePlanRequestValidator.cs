using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.SchedulePlan;

public class CreateSchedulePlanRequestValidator : AbstractValidator<CreateSchedulePlanRequest>
{
    public CreateSchedulePlanRequestValidator()
    {
        RuleFor(x => x.PlanNumber)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.CodeMaxLength);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);

        RuleFor(x => x.WarehouseId)
            .NotEmpty();

        RuleFor(x => x.PlanningStartDateUtc)
            .NotEmpty();

        RuleFor(x => x.PlanningEndDateUtc)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.PlanningStartDateUtc);

        RuleFor(x => x.GenerationMode)
            .InclusiveBetween(1, 3);

        RuleFor(x => x.SchedulingStrategy)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.TimeZoneMaxLength);
    }
}
