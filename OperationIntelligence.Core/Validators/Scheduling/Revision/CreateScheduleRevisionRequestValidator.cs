using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Revision;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Revision;

public class CreateScheduleRevisionRequestValidator : AbstractValidator<CreateScheduleRevisionRequest>
{
    public CreateScheduleRevisionRequestValidator()
    {
        RuleFor(x => x.SchedulePlanId).NotEmpty();

        RuleFor(x => x.RevisionNo)
            .GreaterThan(0);

        RuleFor(x => x.RevisionType)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.ChangeSummary)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.DescriptionMaxLength);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);

        RuleFor(x => x.SnapshotJson)
            .MaximumLength(SchedulingValidationConstants.JsonMaxLength);
    }
}
