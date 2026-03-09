using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class CreateScheduleOperationResourceOptionRequestValidator : AbstractValidator<CreateScheduleOperationResourceOptionRequest>
{
    public CreateScheduleOperationResourceOptionRequestValidator()
    {
        RuleFor(x => x.ScheduleOperationId).NotEmpty();
        RuleFor(x => x.ResourceId).NotEmpty();

        RuleFor(x => x.ResourceType)
            .InclusiveBetween(1, 6);

        RuleFor(x => x.PreferenceRank)
            .GreaterThan(0);

        RuleFor(x => x.EfficiencyFactor)
            .GreaterThan(0);

        RuleFor(x => x.SetupPenaltyMinutes)
            .GreaterThanOrEqualTo(0);
    }
}
