using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.SchedulePlan;

public class PublishSchedulePlanRequestValidator : AbstractValidator<PublishSchedulePlanRequest>
{
    public PublishSchedulePlanRequestValidator()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);

        RuleFor(x => x.ApprovedBy)
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);
    }
}
