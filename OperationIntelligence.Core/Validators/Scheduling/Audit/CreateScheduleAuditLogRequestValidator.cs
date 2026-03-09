using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Audit;
namespace OperationIntelligence.Core.Validators.Scheduling.Audit;

public class CreateScheduleAuditLogRequestValidator : AbstractValidator<CreateScheduleAuditLogRequest>
{
    public CreateScheduleAuditLogRequestValidator()
    {
        RuleFor(x => x.EntityName)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.EntityId)
            .NotEmpty();

        RuleFor(x => x.ActionType)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.ChangedFieldsJson)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.JsonMaxLength);

        RuleFor(x => x.OldValuesJson)
            .MaximumLength(SchedulingValidationConstants.JsonMaxLength);

        RuleFor(x => x.NewValuesJson)
            .MaximumLength(SchedulingValidationConstants.JsonMaxLength);

        RuleFor(x => x.Source)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.ShortNameMaxLength);

        RuleFor(x => x.Reason)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);

        RuleFor(x => x.CorrelationId)
            .MaximumLength(SchedulingValidationConstants.CorrelationIdMaxLength);
    }
}
